using System.Runtime.InteropServices.JavaScript;
using NewPlasmaDonorsAPI.Dto;
using NewPlasmaDonorsAPI.Dto.Dashboard;
using NewPlasmaDonorsAPI.Models;
using NewPlasmaDonorsAPI.Dto;
using PlasmaDonorAPI.Dto; // Ensure the TopInfluencersinfo class is in this namespace


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
        List<Tuple<string,int>> GetProfilesByStateByHomeCenter(int hmcId);
        List<Tuple<string,int>> GetInfuencersByOccupationByHomeCenter(int hmcId);
        List<TopInfluencerInfo> GetTopInfluencers();
        List<Tuple<string, int>> GetInfluencersByRelByHomeCenter(int hmcId);
        List<Tuple<string, int>> GetInfluencersByEduByHomeCenter(int hmcId);
        List<Tuple<string,int>> GetProfilesByState();
        List<Tuple<string, int>> GetInfuencersByOccupation();
        List<Tuple<string, int>> GetInfuencersByRel();
        List<Tuple<string, int>> GetInfuencersByEdu();


    }
}

