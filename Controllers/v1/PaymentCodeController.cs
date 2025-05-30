﻿using IrisPayments.Helpers.Interfaces;
using IrisPayments.Models;
using Microsoft.AspNetCore.Mvc;
using Payments.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace IrisPayments.Controllers.v1;
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[ProducesErrorResponseType(typeof(ProblemDetails))]
[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class PaymentCodeController : ControllerBase {
    private readonly IPaymentCodeHelper _paymentCodeHelperService;
    private readonly IPaymentLogsHelper _paymentLogsHelperService;

    public PaymentCodeController(IPaymentCodeHelper paymentCodeHelperService, IPaymentLogsHelper paymentLogsHelperService) {
        _paymentCodeHelperService = paymentCodeHelperService;
        _paymentLogsHelperService = paymentLogsHelperService;
    }

    [ApiKey]
    [HttpPost("Create")]
    [SwaggerResponse(StatusCodes.Status201Created, "The created Payment Code Object", Type = typeof(CreatePaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreatePaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Create([FromBody] CreatePaymentCodeRequest request) {
        var response = await _paymentCodeHelperService.Upsert(request);
        return Ok(response);
    }

    [ApiKey]
    [HttpPost("Check")]
    [SwaggerResponse(StatusCodes.Status201Created, "The created Payment Code Object", Type = typeof(CheckPaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CheckPaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Check([FromBody] CheckPaymentCodeRequest request) {
        var result = await _paymentCodeHelperService.CheckPaymentCode(request);
        if(result == null) {
            return NotFound();
        }
        return Ok(result);
    }

    [ApiKey]
    [HttpGet("Info/{code}")]
    [SwaggerResponse(StatusCodes.Status200OK, "The Payment Code Object", Type = typeof(CheckPaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CheckPaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Info([FromRoute] string code) {
        var result = await _paymentCodeHelperService.GetPaymentCode(code);
        if(result == null) {
            return NotFound();
        }
        return Ok(result);
    }

    [ApiKey]
    [HttpGet("History/{code}")]
    [SwaggerResponse(StatusCodes.Status200OK, "The Transaction history", Type = typeof(HistoryPaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HistoryPaymentCodeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> History([FromRoute] string code) {
        var result = await _paymentLogsHelperService.GetLogHistory(code);
        if(result == null) {
            return NotFound();
        }
        return Ok(result);
    }

    [BasicAuthorize]
    [HttpPost("Request")]
    [SwaggerResponse(StatusCodes.Status200OK, "DPG_ON Payment request", Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> PaymentRequest([FromBody] PaymentRequest request) {
        var result = await _paymentCodeHelperService.PaymentRequest(request);
        return Ok(result);
    }
}