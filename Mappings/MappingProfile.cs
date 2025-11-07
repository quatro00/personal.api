using AutoMapper;
using Personal.UI.Models.Domain;
using Personal.UI.Models.DTO.Concepto;
using Personal.UI.Models.DTO.Organizacion;

namespace Personal.UI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Organizacion
            CreateMap<Organizacion, OrganizacionDto>();
            CreateMap<CrearOrganizacionDto, Organizacion>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                  .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                 ;
            CreateMap<UpdateOrganizacionDto, Organizacion>()
                ;

            //Concepto
            CreateMap<Concepto, ConceptoDto>()
                .ForMember(dest => dest.TipoConcepto, opt => opt.MapFrom(src => src.TipoConcepto.Descripcion));
            CreateMap<CrearConceptoDto, Concepto>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                  .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                 ;
            

        }
    }
}


