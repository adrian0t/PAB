using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Entities 
{
    public class Project
    {
        [Key]
        public Guid Id { get; private set; } 

        [Required]
        [MaxLength(10)]
        public string Key { get; private set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; private set; }

        public ICollection<UserProject>? UserProjects { get; set; } = new List<UserProject>();
        public ICollection<TaskItem>? Tasks { get; set; } = new List<TaskItem>();
        
        private Project() {} 

        public Project(string key, string name)
        {
            this.Id = Guid.NewGuid();
            this.Key = key;
            this.Name = name;

            //Name = name;
        }
    }
    public static class ProjectExtensions
    {
        public static string GenerateNextTaskKey(this Project project)
        {
            var count = project.Tasks?.Count ?? 0;
            return $"{project.Key}-{count + 1}";
        }
    }
}