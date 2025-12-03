using AutoMapper;
using Book_Lending_System.Application.Abstraction.DTOs.Books;
using Book_Lending_System.Core.Entities.Books;

namespace Book_Lending_System.Application.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDTO>()
                .ReverseMap();

            CreateMap<Book,CreateBookDTO>()
                .ReverseMap();

            CreateMap<Book, UpdateBookDTO>()
                .ReverseMap();

            CreateMap<BorrowRecord,BorrowBook>()
                .ReverseMap();
        }
    }
}
