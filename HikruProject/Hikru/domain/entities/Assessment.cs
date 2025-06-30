using HikruCodeChallenge.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikruCodeChallenge.Domain.Entities;

[Table("ASSESSMENTS")]
public class Assessment
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("TITLE")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    [Column("DESCRIPTION")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("LOCATION")]
    public string Location { get; set; } = string.Empty;

    [Column("STATUS")]
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Draft;

    [Required]
    [Column("RECRUITER_ID")]
    public int RecruiterId { get; set; }

    [Required]
    [Column("DEPARTMENT_ID")]
    public int DepartmentId { get; set; }

    [Required]
    [Column("BUDGET", TypeName = "decimal(18,2)")]
    public decimal Budget { get; set; }

    [Column("CLOSING_DATE")]
    public DateTime? ClosingDate { get; set; }

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Column("UPDATED_DATE")]
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
