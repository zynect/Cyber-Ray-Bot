using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Services;

namespace Dbot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // ~say hello -> hello
        [Command("say")]
        [Summary("Echos a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }
    }

    // Create a module with the 'yt' prefix
    [Group("yt")]
    public class YouTube : ModuleBase<SocketCommandContext>
    {
        [Command("search")]
        [Summary("Searches on YouTube.")]
        [Alias("s")]
        public async Task SearchAsync([Remainder] [Summary("The string to search for.")] string str)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Credentials.YouTubeApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet,id");
            searchListRequest.Q = str;
            searchListRequest.MaxResults = 1;

            var searchListResponse = await searchListRequest.ExecuteAsync();
            if (searchListResponse.Items.Count < 1)
            {
                await ReplyAsync("No results were returned for this query.");
                return;
            }

            var searchResult = searchListResponse.Items[0];

            switch (searchResult.Id.Kind)
            {
                case "youtube#video":
                    await ReplyAsync("Video: "+ searchResult.Snippet.Title + "\nURL: https://youtube.com/watch?v=" + searchResult.Id.VideoId);
                    break;

                case "youtube#channel":
                    await ReplyAsync("Channel: " + searchResult.Snippet.Title + "\nURL: https://youtube.com/channel/" + searchResult.Id.ChannelId);
                    break;

                case "youtube#playlist":
                    await ReplyAsync("Playlist: " + searchResult.Snippet.Title + "\nURL: https://youtube.com/playlist?list=" + searchResult.Id.PlaylistId);
                    break;
            };
        }

        [Command("video")]
        [Summary("Searches for videos on YouTube.")]
        [Alias("vid", "v")]
        public async Task VideoAsync([Remainder] [Summary("The video to search for.")] string video)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Credentials.YouTubeApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet,id");
            searchListRequest.Q = video;
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "video";

            var searchListResponse = await searchListRequest.ExecuteAsync();
            if (searchListResponse.Items.Count < 1)
            {
                await ReplyAsync("No results were returned for this query.");
                return;
            }

            var searchResult = searchListResponse.Items[0];
            
            await ReplyAsync("Video: " + searchResult.Snippet.Title + "\nURL: https://youtube.com/watch?v=" + searchResult.Id.VideoId);
        }

        [Command("channel")]
        [Summary("Searches for channels on YouTube.")]
        [Alias("chan", "c")]
        public async Task ChannelAsync([Remainder] [Summary("The channel to search for.")] string channel)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Credentials.YouTubeApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet,id");
            searchListRequest.Q = channel;
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "channel";

            var searchListResponse = await searchListRequest.ExecuteAsync();
            if (searchListResponse.Items.Count < 1)
            {
                await ReplyAsync("No results were returned for this query.");
                return;
            }

            var searchResult = searchListResponse.Items[0];

            await ReplyAsync("Channel: " + searchResult.Snippet.Title + "\nURL: https://youtube.com/channel/" + searchResult.Id.ChannelId);
        }

        [Command("playlist")]
        [Summary("Searches for videos on YouTube.")]
        [Alias("list", "p")]
        public async Task PlaylistAsync([Remainder] [Summary("The playlist to search for.")] string playlist)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Credentials.YouTubeApiKey,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet,id");
            searchListRequest.Q = playlist;
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "playlist";

            var searchListResponse = await searchListRequest.ExecuteAsync();
            if (searchListResponse.Items.Count < 1)
            {
                await ReplyAsync("No results were returned for this query.");
                return;
            }

            var searchResult = searchListResponse.Items[0];

            await ReplyAsync("Playlist: " + searchResult.Snippet.Title + "\nURL: https://youtube.com/playlist?list=" + searchResult.Id.PlaylistId);
        }
    }
}
