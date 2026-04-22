using CskMasala.Retail.Application.Mappings;
using CskMasala.Retail.Contracts;
using CskMasala.Retail.Domain;
using CskMasala.Shared.Repositories;
using MediatR;

namespace CskMasala.Retail.Application.Commands;

public record CreateCategoryCommand(string Name, string? Description, string? ImageUrl) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler(IWriteRepository<Category> write)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };
        await write.AddAsync(category, ct);
        await write.SaveChangesAsync(ct);
        return category.ToDto();
    }
}
