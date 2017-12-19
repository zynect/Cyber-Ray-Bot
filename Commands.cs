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

        [Command("help")]
        [Summary("Links to the website.")]
        public async Task Help()
        {
            await ReplyAsync("https://s3.us-east-2.amazonaws.com/cyberray/index.html");
        }

        [Command("opgg")]
        [Summary("Prints out URL for op.gg")]
        public async Task SayStats([Remainder][Summary("Stats to display")] string text)
        {
            String[] textArray = text.Split(' ');
            string region = textArray[0];
            string userName = textArray[1];
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("http://");
                if (!region.Equals("kr"))
                {
                    sb.Append(region + ".");
                }
                else
                {
                    sb.Append("www.");
                }
                sb.Append("op.gg/summoner/userName=" + userName);
                for (int i = 2; i < textArray.Length; i++)
                {
                    sb.Append("+" + textArray[i]);
                }
                await ReplyAsync(sb.ToString());
            }
            catch (Exception)
            {
                await ReplyAsync("Username not found");
            }
        }

        [Command("8ball")]
        [Summary("Gives an 8ball-like response to a query.")]
        public async Task EightBall([Remainder][Summary("The question")] string query = null)
        {
            Random rand = new Random();
            string str;
            if (String.IsNullOrEmpty(query))
            {
                str = "Uh, were you trying to ask me something?";
            }
            else
            {
                string[] strings = {"Maybe so.",
                                "Who knows? I sure as hell don't.",
                                "Yeah, sure, whatever.",
                                "Fo' sho', bro.",
                                "Sometime in the distant future",
                                "Try again later.",
                                "Try again never.",
                                "Now why would you ask that?",
                                "No, you dumb shit.",
                                "Hell yeah!",
                                "Maybe, if you're drunk enough.",
                                "Hmm... Maybe with a little luck.",
                                "I'll tell you, after my nap.",
                                "You couldn't pay me to tell you.",
                                "-_-",
                                "JESUS CHRIST NO",
                                "Why not?",
                                "One million percent yes.",
                                "I've never been more sure of anything in my short, robotic life.",
                                "I wish I could rewind time and stop you from asking me that.",
                                "Lemme sleep on it."};
                str = strings[rand.Next(0, strings.Length)];
            }

            await ReplyAsync(Context.User.Mention + ' ' + str);
        }

        [Command("google")]
        [Summary("Googles something for you")]
        public async Task Google([Remainder][Summary("The question")] string query = null)
        {
            string str;
            if (String.IsNullOrEmpty(query))
            {
                str = "Uh, were you trying to ask me something?";
            }
            else
            {
                query = query.Replace(' ', '+');
                str = "http://lmgtfy.com/?q=" + query;
            }

            await ReplyAsync(str);
        }

        [Command("bing")]
        [Summary("Bings something for you")]
        public async Task Bing([Remainder][Summary("The question")] string query = null)
        {
            string str;
            if (String.IsNullOrEmpty(query))
            {
                str = "Uh, were you trying to ask me something?";
            }
            else
            {
                query = query.Replace(' ', '+');
                str = "http://lmgtfy.com/?s=b&q=" + query;
            }

            await ReplyAsync(str);
        }

        [Command("mention")]
        [Summary("mentions a channel")]
        public async Task Mention([Remainder][Summary("The channel")] string query = null)
        {
            string str = "";
            if (String.IsNullOrEmpty(query))
            {
                str = "Uh, were you trying to ask me something?";
            }
            else
            {
                var channels = Context.Guild.VoiceChannels;
                foreach (var chnl in channels)
                {
                    if (chnl.Name.Equals(query))
                    {
                        if (chnl.Users.Count > 0)
                        {
                            foreach (var person in chnl.Users)
                            {
                                str += Context.Guild.GetUser(person.Id).Mention + " ";
                            }
                        }
                        else
                        {
                            str = "There's no one in the channel.";
                        }
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(str))
            {
                str = "I can't find the channel you wanted, keep in mind the search is case sensitive.";
            }

            await ReplyAsync(str);
        }

        [Command("emotion")]
        [Summary("Gives a random emotion.")]
        public async Task Emotion([Remainder][Summary("The request")] string query = null)
        {
            Random rand = new Random();
            string target;
            string str;
            string[] emotions = {"angry.",
                            "sad...",
                            "happy!",
                            "disappointed.",
                            "furious!",
                            "ambivalent.",
                            "disinterested.",
                            "excited!",
                            "exhausted...",
                            "nervous.",
                            "tired.",
                            "disgusted. *Ugh*",
                            "surprised!",
                            "embarrassed...",
                            "scared.",
                            "terrified!",
                            "angsty!",
                            "like shit...",
                            "exuberant!",
                            "vindicated.",
                            "powerful!",
                            "curmudgeonly.",
                            "dead inside.",
                            "great!",
                            "nothing.",
                            "***horny***.",
                            "hyper!"};
            if (String.IsNullOrEmpty(query))
                target = "I feel ";
            else
                target = "They feel ";

            str = target + emotions[rand.Next(0, emotions.Length)];

            await ReplyAsync(str);
        }

        [Command("prefix")]
        [Summary("Changes the prefix that summons the bot.")]
        public async Task Prefix([Summary("The prefix")] char prefix)
        {
            if ((Context.User as SocketGuildUser).GuildPermissions.ManageNicknames)
            {
                if(prefix == '#' || prefix == '@' || prefix == '<')
                {
                    await ReplyAsync($"I ain't changing my prefix to *that*.");
                    return;
                }
                var user = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
                await user.ModifyAsync(x => {
                    x.Nickname = $"{prefix}Cyber Ray";
                });
                await ReplyAsync($"Changed command prefix to {prefix}.");
            }
            else
            {
                AdminRestrictedCommand();
            }
        }

        [Command("teams")]
        [Summary("Makes two teams out of a voice channel.")]
        public async Task Teams([Summary("The number of teams")] int teams)
        {
            if ((Context.User as SocketGuildUser).VoiceChannel == null)
            {
                await ReplyAsync($"Now who in the hell do you expect me to team you up with, {Context.User.Mention}? Get in a voice chat first!");
                return;
            }

            var users = (Context.User as SocketGuildUser).VoiceChannel.Users;

            if (users.Count < teams)
            {
                if (users.Count == 1)
                {
                    await ReplyAsync($"Any particular reason you want to split yourself in {teams}, {Context.User.Mention}? Sounds mighty painful.");
                }
                else
                {
                    await ReplyAsync($"You can't make {teams} teams out of {users.Count} users.");
                }
                return;
            }
            else if (teams < 2)
            {
                await ReplyAsync($"...You kidding me right now, {Context.User.Mention}? " + ((teams == 1) ? "There's no point in making only one team." : $"I can't exactly make {teams} teams."));
                return;
            }

            List<ulong>[] teamLists = new List<ulong>[teams];

            for(int i = 0; i < teams; i++)
            {
                teamLists[i] = new List<ulong>();
            }

            Random r = new Random();
            int currentTeam = 0;

            foreach (int i in Enumerable.Range(0, users.Count).OrderBy(x => r.Next()))
            {
                teamLists[currentTeam++].Add(users.ElementAt(i).Id);
                currentTeam %= teams;
            }

            string[] nameCalling = {"maggots",
                                    "babies",
                                    "yankees",
                                    "losers",
                                    "shut up, all of ya'",
                                    "dumbfucks",
                                    "shitstains",
                                    "you beautiful people"};
            string funnyString = nameCalling[r.Next(0, nameCalling.Length)];

            await ReplyAsync($"All right, {funnyString}! Let's play a game together.");
            for (int i = 0; i < teams; i++)
            {
                string teamString = $"Team {i + 1}! Here are your teammates:\n";
                foreach (ulong ID in teamLists[i])
                {
                    teamString += Context.Guild.GetUser(ID).Mention + '\n';
                }
                await ReplyAsync(teamString);
            }
        }

        public async void AdminRestrictedCommand()
        {
            await ReplyAsync($"Don't you start with me, {Context.User.Mention}.");
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
