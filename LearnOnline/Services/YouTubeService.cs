namespace LearnOnline.Services
{
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class YouTubeApiService
    {
        private readonly string apiKey = "AIzaSyCDQ-tz_6h5hjMl9iqWxHOfWPKJFO_-zDM";

        public async Task<List<(string VideoId, string Title)>> GetVideoDetailsFromPlaylistAsync(string playlistId)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
            });

            var playlistItemsRequest = youtubeService.PlaylistItems.List("contentDetails,snippet");
            playlistItemsRequest.PlaylistId = playlistId;
            playlistItemsRequest.MaxResults = 50;

            var videoDetails = new List<(string VideoId, string Title)>();
            string nextPageToken = null;

            do
            {
                playlistItemsRequest.PageToken = nextPageToken;
                var playlistItemsResponse = await playlistItemsRequest.ExecuteAsync();

                foreach (var item in playlistItemsResponse.Items)
                {
                    var videoId = item.ContentDetails.VideoId;
                    var title = item.Snippet.Title;
                    videoDetails.Add((videoId, title));
                }

                nextPageToken = playlistItemsResponse.NextPageToken;
            } while (nextPageToken != null);

            return videoDetails;
        }
    }
}
