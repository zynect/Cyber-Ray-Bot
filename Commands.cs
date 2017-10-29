using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

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
        [Command("stats")]
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
    }
}
