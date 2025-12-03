using Book_Lending_System.Application.Features.Books.Commands.Models;
using FluentValidation;

namespace Book_Lending_System.Application.Features.Books.Commands.Validators
{
    public class AddBookValidator:AbstractValidator<AddBookCommand>
    {
        public AddBookValidator()
        {
            ApplyValidations();
        }
        public void ApplyValidations()
        {
            RuleFor(x => x.CreateBookDTO.Title)
                .NotEmpty().WithMessage("Title is required");

            RuleFor(x => x.CreateBookDTO.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.CreateBookDTO.Author)
                .NotEmpty().WithMessage("Author name is required");
        }
    }
}
