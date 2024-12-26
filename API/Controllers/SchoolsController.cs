
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CommonClasses.Models;
using API.DTOs;
using API.Services;
using CommonClasses.Enums;

namespace API.Controllers;

[ApiController]
[Route("/[controller]")]
public class SchoolsController : ControllerBase
{
    private  Validations _schoolValidations;
    private readonly SchoolWorkerService _schoolService;
    private readonly WebhookService _webhookService;
    private readonly ILogger<SchoolsController> _logger;
  
    
    public SchoolsController(ILogger<SchoolsController> logger) 
    {
        _schoolValidations = new Validations();
        _schoolService = new SchoolWorkerService();
        _webhookService = new WebhookService();
        _logger = logger;
       
    }

    private async Task activateWebhookWorker(string webhooks_url,string taskName,string taskResult)
    {
        if (webhooks_url != null)
        {
            await _webhookService.InsertTaskIntoQueueAsync(webhooks_url,taskName,taskResult);
        }
    }
    
    private async Task<IActionResult> badRequestHandle(Exception exception, string webhooks_url, string taskName)
    {
        _logger.LogError($"There was an error:{exception.Message}");
        await activateWebhookWorker(webhooks_url,taskName,"Failed");
        return BadRequest(exception.Message);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSchools([FromQuery]RequestDto requestDto)
    {
        try
        {
            string taskId = await _schoolService.InsertTaskIntoQueueAsync(TaskType.GetSchools);
            _logger.LogInformation($"API: Getting all Schools id: {taskId}");
            var results = await _schoolService.GetTaskResults(taskId,requestDto.CancellationToken);
            if (results != null)
            {
                await activateWebhookWorker(requestDto.webhooks_url,"Get All Schools","Succeed");
            }
            
            return results.Count > 0 ? Ok(results) : NotFound("There is no schools yet!");
        }
        catch (Exception e)
        {
           return await badRequestHandle(e,requestDto.webhooks_url,"Get All Schools");
        }
        
        
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSchoolFromId([FromQuery]RequestDto requestDto,int id)
    {
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(id,"Id");
        if (!validationDisplay.IsValid)
        {
            return await badRequestHandle(new Exception("Wrong Id , it must be bigger than 0"), requestDto.webhooks_url,
                "Search School by Id");
        }

        try
        {
            string taskId = await _schoolService.InsertTaskIntoQueueAsync(id, TaskType.GetSchoolFromId);
            _logger.LogInformation($"API: Getting a Schools by id:{id} taskId:{taskId}");
            var result = await _schoolService.GetTaskResults(taskId, requestDto.CancellationToken);
            _logger.LogInformation($"API: Finished getting a School by id:{id}, taskId:{taskId}");
            if (result != null)
            {
                await activateWebhookWorker(requestDto.webhooks_url,"Get By Id","Succeed");
            }
            return result.Count > 0 ? Ok(result) : NotFound("There is no school found!");
        }
        catch (Exception e)
        {
            return await badRequestHandle(e,requestDto.webhooks_url,"Get a school by id");
        }
        

    }

    [HttpGet("name/{schoolName}")]
    public async Task<IActionResult> GetSchoolFromName([FromQuery]RequestDto requestDto,string schoolName)
    {
        try
        {
            string taskId = await _schoolService.InsertTaskIntoQueueAsync(schoolName);
            _logger.LogInformation($"API: Getting a Schools by name:{schoolName} taskId:{taskId}");
            var result = await _schoolService.GetTaskResults(taskId, requestDto.CancellationToken);
            _logger.LogInformation($"API: Finished getting  Schools by name , taskId:{taskId}");
            if (result != null)
            {
                await activateWebhookWorker(requestDto.webhooks_url, "Get By Name", "Succeed");
            }
           
            return result.Count > 0 ? Ok(result) : NotFound("There is no school found!");
        }
        catch (Exception e)
        {
            return await badRequestHandle(e,requestDto.webhooks_url,"Get Schools By name");
        }
    }
    
    [HttpGet("district/{districtId}")]
    public async Task<IActionResult> GetSchoolsFromDistrict([FromQuery]RequestDto requestDto,int districtId)
    {
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(districtId, "DistrictId");
        if (!validationDisplay.IsValid)
        {
            return await badRequestHandle(new Exception(validationDisplay.ToString()),requestDto.webhooks_url,"Get Schools By District");
        }

        try
        {
            string taskId = await _schoolService.InsertTaskIntoQueueAsync(districtId, TaskType.GetSchoolByDistrict);
            _logger.LogInformation($"API: Getting a Schools by District Id:{districtId} taskId:{taskId}");
            var result = await _schoolService.GetTaskResults(taskId, requestDto.CancellationToken);
            _logger.LogInformation($"API: Finished getting School by District Id:{districtId}, taskId:{taskId}");
            if (result != null)
            {
                await activateWebhookWorker(requestDto.webhooks_url, "Get By District", "Succeed");
            }
            
            return result.Count > 0 ? Ok(result) : NotFound("There is no schools found in this district!");
        }
        catch (Exception e)
        {
            return await badRequestHandle(e,requestDto.webhooks_url,"Get Schools By District Id");
        }
        
    }

    [HttpPost("addSchool")]
    public async Task<IActionResult> AddSchool([FromQuery]SchoolDto schoolDto,[FromQuery]RequestDto requestDto)
    {
        var validationDisplays = _schoolValidations.CheckNewSchool(schoolDto);
        if (validationDisplays != null)
        {
            return await badRequestHandle(new Exception(JsonSerializer.Serialize(validationDisplays)),requestDto.webhooks_url,"Add a School");
        }
        
        var newSchool = new School()
        {
                Name = schoolDto.Name,
                DistrictId = schoolDto.DistrictId,
                LicenseId = schoolDto.LicenseId,
                CreatedAt = DateTime.Now.ToString(),
        };

        try
        {
            string taskId = await _schoolService.InsertTaskIntoQueueAsync(newSchool, TaskType.AddNewSchool);
            _logger.LogInformation($"API: Adding new School , taskId:{taskId}");
            var result = await _schoolService.CheckIfTaskHasCompleted(taskId);
            _logger.LogInformation($"API: Finished adding new School , taskId:{taskId}");
            await _webhookService.InsertTaskIntoQueueAsync(requestDto.webhooks_url, "Add New School", "Succeed");
            return result ? Ok("School has been Added!") : BadRequest("There was an error adding school");
        }
        catch (Exception e)
        {
            return await badRequestHandle(e,requestDto.webhooks_url,"Add a School");
        }
        
    }
    
    [HttpGet("deleteSchool/{schoolId}")]
    public async Task<IActionResult> DeleteSchool([FromQuery]RequestDto requestDto,int schoolId)
    { 
        ValidationDisplay validationDisplay = _schoolValidations.CheckNumericFieldValidations(schoolId, "Id");
        if (!validationDisplay.IsValid)
        {
            return BadRequest("Id must be greater than 0");
        }

        try
        {
            string taskId = await _schoolService.InsertTaskIntoQueueAsync(schoolId, TaskType.DeleteSchool);
            _logger.LogInformation($"API: Deleting a School by id:{schoolId} taskId:{taskId}");
            var result = await _schoolService.CheckIfTaskHasCompleted(taskId);
            _logger.LogInformation($"API: Finished Deleting a School by id:{schoolId} taskId:{taskId}");
            await _webhookService.InsertTaskIntoQueueAsync(requestDto.webhooks_url, "Delete School", "Succeed");
            return result ? Ok("this school has been deleted!") : BadRequest("This school doesnt exist");
        }
        catch (Exception e)
        {
            return await badRequestHandle(e,requestDto.webhooks_url,"Delete a School");
        }
        
        
    }
    
    [HttpPost("updateSchool/{schoolId}")]
    public async Task<IActionResult> UpdateSchool([FromQuery]SchoolUpdateDto schoolUpdateDto,[FromQuery]RequestDto requestDto,int schoolId)
    {
        var validations = _schoolValidations.CheckUpdatedSchool(schoolUpdateDto, schoolId);
        if (validations.Count > 0)
        {
            return BadRequest(validations);
        }

        try
        {
            string taskId =
                await _schoolService.InsertTaskIntoQueueAsync(schoolId, schoolUpdateDto, TaskType.UpdateSchool);
            _logger.LogInformation($"API: Updating a School by id:{schoolId} taskId:{taskId}");
            var result = await _schoolService.CheckIfTaskHasCompleted(taskId);
            _logger.LogInformation($"API: Finished update a School by id:{schoolId} taskId:{taskId}");
            await _webhookService.InsertTaskIntoQueueAsync(requestDto.webhooks_url, "Update School", "Succeed");
            return result ? Ok("School has been updated!") : BadRequest("This School doesnt exist");
        }
        catch (Exception e)
        {
            return await badRequestHandle(e,requestDto.webhooks_url,"Update a School");
        }
     
        
    }
        
}