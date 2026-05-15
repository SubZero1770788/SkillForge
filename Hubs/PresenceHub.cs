using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using quiz_project.Database;
using quiz_project.Entities;

namespace quiz_project.Hubs
{
    public class PresenceHub(UserManager<User> userManager, QuizDb context) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = await userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                user.IsLoggedIn = true;
                await userManager.UpdateAsync(user);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                user.IsLoggedIn = false;
                await userManager.UpdateAsync(user);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}