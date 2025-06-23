using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class MangaBoxService : IMangaBoxService
    {
        private readonly IUnitOfWork _uniUnitOfWork;
        public MangaBoxService(IUnitOfWork unitOfWork)
        {
            _uniUnitOfWork = unitOfWork;
        }

        public async Task<MangaBox> AddAsync(MangaBox mangaBox) => await _uniUnitOfWork.MangaBoxRepository.AddAsync(mangaBox);
        public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync() => await _uniUnitOfWork.MangaBoxRepository.GetAllWithDetailsAsync();
        public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id) => await _uniUnitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(id);
    }
}
