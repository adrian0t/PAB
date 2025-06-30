using System.Threading.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services
{
    public class ProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Guid> CreateProjectAsync(NewProjectDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Project name cannot be empty.");
            }

            var project = new Project(dto.Name, dto.Name);

            await _projectRepository.AddAsync(project);

            return project.Id;
        }
    }
}