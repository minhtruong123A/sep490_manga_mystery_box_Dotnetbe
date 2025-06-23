using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using DataAccessLayers.Interface;
using Services.Helper.Supabase;
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
        private readonly ISupabaseStorageHelper _supabaseStorageHelper;
        public MangaBoxService(IUnitOfWork unitOfWork, ISupabaseStorageHelper supabaseStorageHelper)
        {
            _uniUnitOfWork = unitOfWork;
            _supabaseStorageHelper = supabaseStorageHelper;
        }

        public async Task<MangaBox> AddAsync(MangaBox mangaBox) => await _uniUnitOfWork.MangaBoxRepository.AddAsync(mangaBox);

        public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync()
        {
            var boxes = await _uniUnitOfWork.MangaBoxRepository.GetAllWithDetailsAsync();
            await Task.WhenAll(boxes
                .Where(b => !string.IsNullOrEmpty(b.UrlImage))
                .Select(async b =>
                {
                    if (!string.IsNullOrEmpty(b.UrlImage)) b.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(b.UrlImage);    
                }));

            return boxes;
        }

        public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id)
        {
            var box = await _uniUnitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(id);
            if (box?.UrlImage is { Length: > 0 }) box.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(box.UrlImage);
            if (box?.Products != null)
            {
                var signTasks = box.Products
                    .Where(p => !string.IsNullOrEmpty(p.UrlImage))
                    .Select(async p => p.UrlImage = await _supabaseStorageHelper.CreateSignedUrlAsync(p.UrlImage));
                await Task.WhenAll(signTasks);
            }

            return box;
        }
    }
}
