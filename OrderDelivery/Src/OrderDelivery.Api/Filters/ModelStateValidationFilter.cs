using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderDelivery.Application.DTOs.Responses;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace OrderDelivery.Api.Filters
{
    public class ModelStateValidationFilter : IActionFilter
    {
        private readonly ILogger<ModelStateValidationFilter> _logger;

        public ModelStateValidationFilter(ILogger<ModelStateValidationFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("ModelStateValidationFilter executing for action: {ActionName}", context.ActionDescriptor.DisplayName);
            
            // Get the action arguments to find the DTO
            var actionArguments = context.ActionArguments.Values;
            var dto = actionArguments.FirstOrDefault();
            
            if (dto != null)
            {
                // Try to find and run the appropriate validator
                var validatorType = typeof(IValidator<>).MakeGenericType(dto.GetType());
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
                
                if (validator != null)
                {
                    _logger.LogInformation("Found validator for type: {DtoType}", dto.GetType().Name);
                    
                    // Run the validator
                    var validationResult = validator.Validate(new ValidationContext<object>(dto));
                    
                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning("Validation failed with {ErrorCount} errors", validationResult.Errors.Count);
                        
                        var errors = validationResult.Errors
                            .Select(e => e.ErrorMessage)
                            .ToList();

                        _logger.LogInformation("Validation errors from validator: {Errors}", string.Join(", ", errors));

                        var response = new ApiResponse<object>(
                            null,
                            "Validation failed",
                            false,
                            errors
                        );

                        context.Result = new BadRequestObjectResult(response);
                        _logger.LogInformation("Set BadRequest result with ApiResponse from validator");
                        return;
                    }
                    else
                    {
                        _logger.LogInformation("Validation passed for {DtoType}", dto.GetType().Name);
                    }
                }
                else
                {
                    _logger.LogWarning("No validator found for type: {DtoType}", dto.GetType().Name);
                }
            }
            
            // Fallback to ModelState validation if no validator found or validation passed
            if (context.ModelState.ErrorCount > 0)
            {
                _logger.LogWarning("ModelState has {ErrorCount} errors", context.ModelState.ErrorCount);
                
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                _logger.LogInformation("ModelState validation errors: {Errors}", string.Join(", ", errors));

                var response = new ApiResponse<object>(
                    null,
                    "Validation failed",
                    false,
                    errors
                );

                context.Result = new BadRequestObjectResult(response);
                _logger.LogInformation("Set BadRequest result with ApiResponse from ModelState");
            }
            else
            {
                _logger.LogInformation("ModelState is valid");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("ModelStateValidationFilter executed for action: {ActionName}", context.ActionDescriptor.DisplayName);
        }
    }
}
