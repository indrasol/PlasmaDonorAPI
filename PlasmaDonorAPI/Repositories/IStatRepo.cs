using System.Runtime.InteropServices.JavaScript;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Dto.Dashboard;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Dto; // Ensure the TopInfluencersinfo class is in this namespace


namespace NewPlasmaDonorsAPI.Repositories
{


    public interface IStatRepo
    {
        Task<int> GetDonorCountAsync();
        Task<int> GetInfluencerCountAsync();
        Task<int> GetRecentInfluencerCountAsync(DateTime date);
        Task<int> GetRecentDonorCountAsync(DateTime date);
        // Task<List<TopInfluencersinfo>> GetTopInfluencersAsync();
        List<Tuple<string, int>> GetInfTimeSeries(DateTime date);
        List<Tuple<string, int>> GetDonorTimeSeries(DateTime date);
        //List<Tuple<string, int>> GetProfilesByState();
        List<Tuple<string, int>> GetDonorsByOccupation();
        List<ProfileDto> GetDonorInfDataAsync();
        Task<List<Tuple<long, long?, string, string, string, string>>> GetDonorInfDataByInfIdAsync(List<int> infIds);
        Task<List<Tuple<long?, int>>> GetScoreByInfIdsAsync(List<long> profileIds);
        List<CountDto> GetProfilesByStateByHomeCenter(int hmcId);
        List<CountDto> GetInfuencersByOccupationByHomeCenter(int hmcId);

        List<CountDto> GetInfluencersByRelByHomeCenter(int hmcId);
        List<CountDto> GetInfluencersByEduByHomeCenter(int hmcId);
        List<CountDto> GetProfilesByState();
        List<CountDto> GetInfuencersByOccupation();
        List<CountDto> GetInfuencersByRel();
        List<CountDto> GetInfuencersByEdu();


    }
}

