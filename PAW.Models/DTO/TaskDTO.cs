using System;
using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

public class TaskDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime? DueDate { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("lastModified")]
    public DateTime? LastModified { get; set; }

    [JsonPropertyName("modifiedBy")]
    public string? ModifiedBy { get; set; }

    public static TaskDTO ConvertFrom(PAW.Models.Task task)
    {
        return new TaskDTO
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            LastModified = task.LastModified,
            ModifiedBy = task.ModifiedBy
        };
    }

    public static PAW.Models.Task Convert(TaskDTO dto)
    {
        return new PAW.Models.Task
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status,
            DueDate = dto.DueDate,
            CreatedAt = dto.CreatedAt ?? DateTime.Now,
            LastModified = dto.LastModified,
            ModifiedBy = dto.ModifiedBy
        };
    }
}