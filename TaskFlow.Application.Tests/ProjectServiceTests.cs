using Moq; 
using TaskFlow.Application.Services; 
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;
using Xunit; 

namespace TaskFlow.Application.Tests
{
    public class ProjectServiceTests
    {
        [Fact]
        public async Task CreateProjectAsync_WithValidName_ShouldCallRepositoryAndReturnProjectId()
        {
            var projectRepositoryMock = new Mock<IProjectRepository>();

            var projectService = new ProjectService(projectRepositoryMock.Object);

            var newProjectDto = new NewProjectDTO { Name = "Nowy super projekt" };

            var projectId = await projectService.CreateProjectAsync(newProjectDto);

            Assert.NotEqual(Guid.Empty, projectId);

           
            projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Task CreateProjectAsync_WithEmptyName_ShouldThrowArgumentException()
        {
            var projectRepositoryMock = new Mock<IProjectRepository>();
            var projectService = new ProjectService(projectRepositoryMock.Object);
            var newProjectDto = new NewProjectDTO { Name = "" };

           
            Func<Task> act = () => projectService.CreateProjectAsync(newProjectDto);

            await Assert.ThrowsAsync<ArgumentException>(act);

            projectRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Project>()), Times.Never);
        }
    }
}
