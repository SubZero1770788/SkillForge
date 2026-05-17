using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using quiz_project.Database;
using quiz_project.Entities;

namespace quiz_project.Infrastructure
{
    public static class DbSeeder
    {
        public async static void Initialize(AsyncServiceScope scope, QuizDb context)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Quizzes.Any())
            {
                Console.WriteLine("Database seeding unnecessary - data is already there.");
                return;
            }

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                if (!context.Roles.Any())
                {
                    var roles = new List<Role>{
                        new Role()
                        {
                            Id = 1,
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new Role()
                        {
                            Id = 2,
                            Name = "Moderator",
                            NormalizedName = "MODERATOR"
                        },
                        new Role()
                        {
                            Id = 3,
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new Role()
                        {
                            Id = 4,
                            Name = "Creator",
                            NormalizedName = "CREATOR"
                        }
                    };

                    foreach (Role role in roles)
                    {
                        await roleManager.CreateAsync(role);
                    }
                    await context.SaveChangesAsync();
                }

                var Users = new List<User>
                {
                    new User{
                        UserName = "Michael",
                        Email = "michael@123",
                        IsLoggedIn=false,
                    },
                    new User{
                        UserName = "Sylvia",
                        Email = "sylvia123@o2.org",
                        IsLoggedIn=false,
                    },
                };

                foreach (User user in Users)
                {
                    await userManager.CreateAsync(user, "123123ASD!@#a");
                    await userManager.AddToRoleAsync(user, "User");
                }

                var mod = new User
                {
                    UserName = "moderator",
                    Email = "moderrratormail@o2.org",
                    IsLoggedIn = false,
                };

                await userManager.CreateAsync(mod, "123123ASD!@#a");
                await userManager.AddToRoleAsync(mod, "Moderator");

                await context.SaveChangesAsync();

                var admin = new User
                {
                    UserName = "admin",
                    Email = "adminmail@o2.org",
                    IsLoggedIn = false,
                };

                await userManager.CreateAsync(admin, "123123ASD!@#a");
                await userManager.AddToRoleAsync(admin, "Admin");

                await context.SaveChangesAsync();

                var creator = new User
                {
                    UserName = "creator",
                    Email = "creatormail@o2.org",
                    IsLoggedIn = false,
                };

                await userManager.CreateAsync(creator, "123123ASD!@#a");
                await userManager.AddToRoleAsync(creator, "Creator");

                await context.SaveChangesAsync();

                var Courses = new List<Course>
                {
                    new Course{
                        CourseId = 1,
                        Title = "My first course",
                        Description = "This course has no other meaning",
                        IsPublic = true,
                        UserId = 5
                    },
                    new Course{
                        CourseId = 2,
                        Title = "My second course",
                        Description = "I did not know I can create more of them !",
                        IsPublic = false,
                        UserId = 5
                    }
                };

                await context.Courses.AddRangeAsync(Courses);
                await context.SaveChangesAsync();

                var quizes = new List<Quiz>
                {
                    new Quiz{
                        QuizId = 1,
                        Title = "My first quiz",
                        Description = "This quiz has no other meaning",
                        IsPublic = true,
                        UserId = 1,
                        TotalScore = 200
                    },
                    new Quiz{
                        QuizId = 2,
                        Title = "My second quiz",
                        Description = "I did not know I can create more of them !",
                        IsPublic = false,
                        UserId = 1,
                        TotalScore = 0
                    },
                        new Quiz{
                        QuizId = 3,
                        Title = "Power Metal Music",
                        Description = "Test your knowledge about legendary power metal bands and songs.",
                        IsPublic = true,
                        UserId = 1,
                        TotalScore = 200
                    },
                    new Quiz{
                        QuizId = 4,
                        Title = "European Geography",
                        Description = "How well do you know the countries, cities, and rivers of Europe?",
                        IsPublic = true,
                        UserId = 1,
                        TotalScore = 200
                    },
                    new Quiz{
                        QuizId = 5,
                        Title = "Pokemon",
                        Description = "Prove you're a true Pokémon Master with this quiz!",
                        IsPublic = true,
                        UserId = 1,
                        TotalScore = 200
                    },
                    new Quiz{
                        QuizId = 6,
                        Title = "Amazing Game OST",
                        Description = "Recognize iconic soundtracks from video games.",
                        IsPublic = true,
                        UserId = 1,
                        TotalScore = 200
                    },
                    new Quiz{
                        QuizId = 7,
                        Title = "C# Advanced",
                        Description = "Question C# advanced topics, including async, delegates, and LINQ.",
                        IsPublic = true,
                        UserId = 1,
                        TotalScore = 200
                    }
                };

                await context.Quizzes.AddRangeAsync(quizes);
                await context.SaveChangesAsync();

                var questions = new List<Question>
                {
                    new Question{
                        QuestionId = 1,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the capital of France?"
                    },
                    new Question{
                        QuestionId = 2,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is 2 + 2?"
                    },
                    new Question{
                        QuestionId = 3,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "Who wrote '1984'?"
                    },
                    new Question{
                        QuestionId = 4,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the boiling point of water?"
                    },
                    new Question{
                        QuestionId = 5,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the largest ocean on Earth?"
                    },
                    new Question{
                        QuestionId = 6,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "Who painted the Mona Lisa?"
                    },
                    new Question{
                        QuestionId = 7,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the currency of Japan?"
                    },
                    new Question{
                        QuestionId = 8,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What element has the symbol O?"
                    },
                    new Question{
                        QuestionId = 9,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "Who discovered gravity?"
                    },
                    new Question{
                        QuestionId = 10,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What year did WW2 end?"
                    },
                    new Question{
                        QuestionId = 11,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the square root of 16?"
                    },
                    new Question{
                        QuestionId = 12,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What planet is known as the Red Planet?"
                    },
                    new Question{
                        QuestionId = 13,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "Who invented the telephone?"
                    },
                    new Question{
                        QuestionId = 14,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What language is spoken in Brazil?"
                    },
                    new Question{
                        QuestionId = 15,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What gas do plants absorb from the air?"
                    },
                    new Question{
                        QuestionId = 16,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "Who is the author of 'Harry Potter'?"
                    },
                    new Question{
                        QuestionId = 17,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the tallest mountain in the world?"
                    },
                    new Question{
                        QuestionId = 18,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What does DNA stand for?"
                    },
                    new Question{
                        QuestionId = 19,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "Who was the first man on the moon?"
                    },
                    new Question{
                        QuestionId = 20,
                        QuizId = 1,
                        QuestionScore= 10,
                        Description = "What is the speed of light?"
                    }, new Question{
                        QuestionId = 21,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which band released the album 'Nightfall in Middle-Earth'?"
                    },
                    new Question{
                        QuestionId = 22,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these bands is known for fantasy-themed lyrics?"
                    },
                    new Question{
                        QuestionId = 23,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which band features Tobias Sammet as the lead vocalist?"
                    },
                    new Question{
                        QuestionId = 24,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which country is home to the band Stratovarius?"
                    },
                    new Question{
                        QuestionId = 25,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "What is the genre of the song 'Through the Fire and Flames'?"
                    },
                    new Question{
                        QuestionId = 26,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which band is known for incorporating historical themes into their lyrics?"
                    },
                    new Question{
                        QuestionId = 27,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these bands is primarily symphonic power metal?"
                    },
                    new Question{
                        QuestionId = 28,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Who is known as the 'father of power metal'?"
                    },
                    new Question{
                        QuestionId = 29,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these bands has a keyboardist as a core member?"
                    },
                    new Question{
                        QuestionId = 30,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these albums is by Helloween?"
                    },
                    new Question{
                        QuestionId = 31,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these are power metal bands? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 32,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these albums were released by Rhapsody of Fire? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 33,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which instruments are typically prominent in power metal? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 34,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "What lyrical themes are common in power metal?"
                    },
                    new Question{
                        QuestionId = 35,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of the following bands is known for fast guitar solos?"
                    },
                    new Question{
                        QuestionId = 36,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of these musicians is associated with both Gamma Ray and Helloween?"
                    },
                    new Question{
                        QuestionId = 37,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "What characterizes power metal vocals?"
                    },
                    new Question{
                        QuestionId = 38,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which of the following are subgenres or variations of power metal?"
                    },
                    new Question{
                        QuestionId = 39,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "What is commonly used in power metal artwork?"
                    },
                    new Question{
                        QuestionId = 40,
                        QuizId = 3,
                        QuestionScore= 10,
                        Description = "Which label is known for signing many power metal bands?"
                    },
                     new Question{
                        QuestionId = 41,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "What is the longest river in Europe?"
                    },
                    new Question{
                        QuestionId = 42,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which of the following are landlocked European countries? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 43,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which mountain range separates Europe from Asia?"
                    },
                    new Question{
                        QuestionId = 44,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which country has the most islands in Europe?"
                    },
                    new Question{
                        QuestionId = 45,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which is the smallest country in Europe?"
                    },
                    new Question{
                        QuestionId = 46,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "What is the capital of Slovakia?"
                    },
                    new Question{
                        QuestionId = 47,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which countries border Germany? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 48,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which sea borders Romania?"
                    },
                    new Question{
                        QuestionId = 49,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which European city is split by the Danube River?"
                    },
                    new Question{
                        QuestionId = 50,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which European countries use the Euro? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 51,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "What is the highest mountain in Europe?"
                    },
                    new Question{
                        QuestionId = 52,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which countries are part of Scandinavia?"
                    },
                    new Question{
                        QuestionId = 53,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which country is famous for its fjords?"
                    },
                    new Question{
                        QuestionId = 54,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "What is the capital of Lithuania?"
                    },
                    new Question{
                        QuestionId = 55,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which European capital lies furthest west?"
                    },
                    new Question{
                        QuestionId = 56,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which countries form the Iberian Peninsula?"
                    },
                    new Question{
                        QuestionId = 57,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which river runs through Paris?"
                    },
                    new Question{
                        QuestionId = 58,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which European country is both in Europe and Asia?"
                    },
                    new Question{
                        QuestionId = 59,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "What is the capital of Croatia?"
                    },
                    new Question{
                        QuestionId = 60,
                        QuizId = 4,
                        QuestionScore= 10,
                        Description = "Which country has the city of Bruges?"
                    },
                    new Question{
                        QuestionId = 61,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of these are starter Pokémon in the Kanto region? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 62,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "What type is Gyarados?"
                    },
                    new Question{
                        QuestionId = 63,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which Pokémon games introduced the Johto region?"
                    },
                    new Question{
                        QuestionId = 64,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of these Pokémon are Electric-type? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 65,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which region features the city of Lumiose?"
                    },
                    new Question{
                        QuestionId = 66,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "What is the evolved form of Eevee into a Water-type?"
                    },
                    new Question{
                        QuestionId = 67,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which Pokémon are Legendary from Gen I? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 68,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "What is the main goal in most mainline Pokémon games?"
                    },
                    new Question{
                        QuestionId = 69,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which Pokémon evolves using a Moon Stone? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 70,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of the following are types introduced after Gen I? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 71,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of these games are part of Generation III?"
                    },
                    new Question{
                        QuestionId = 72,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which Pokémon is known for the move 'Splash'?"
                    },
                    new Question{
                        QuestionId = 73,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of the following are version mascots? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 74,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which Pokémon can evolve from Eevee? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 75,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of the following are dual-type Pokémon? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 76,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which Pokémon has a Gigantamax form? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 77,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "What is the primary type of Lucario?"
                    },
                    new Question{
                        QuestionId = 78,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of these are Gym Leader names? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 79,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which of these Pokémon are fossil Pokémon? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 80,
                        QuizId = 5,
                        QuestionScore= 10,
                        Description = "Which are Legendary Pokémon from Gen II?"
                    },
                    new Question{
                        QuestionId = 81,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which games were composed by Nobuo Uematsu? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 82,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Who composed the soundtrack for 'The Elder Scrolls V: Skyrim'?"
                    },
                    new Question{
                        QuestionId = 83,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which of these games are known for their orchestral soundtracks? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 84,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which series is known for the song 'Zelda’s Lullaby'?"
                    },
                    new Question{
                        QuestionId = 85,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "What genre best describes the music in 'DOOM (2016)'?"
                    },
                    new Question{
                        QuestionId = 86,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Who composed music for 'Kingdom Hearts'?"
                    },
                    new Question{
                        QuestionId = 87,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which game is known for the track 'To Zanarkand'?"
                    },
                    new Question{
                        QuestionId = 88,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which composers are known for work on 'NieR: Automata'? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 89,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which game soundtrack includes the song 'Baba Yetu'?"
                    },
                    new Question{
                        QuestionId = 90,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which game series features 'Lifelight' as its theme?"
                    },
                    new Question{
                        QuestionId = 91,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which of these games are praised for dynamic, reactive music systems? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 92,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which games have tracks composed by Koji Kondo? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 93,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which game is known for the track 'Megalovania'?"
                    },
                    new Question{
                        QuestionId = 94,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which composer worked on the Metal Gear Solid series?"
                    },
                    new Question{
                        QuestionId = 95,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which of the following games feature notable boss music? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 96,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which game's soundtrack includes 'The Opened Way'?"
                    },
                    new Question{
                        QuestionId = 97,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Who composed the soundtrack for 'Journey'?"
                    },
                    new Question{
                        QuestionId = 98,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which of these have won awards for best video game score? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 99,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which of these indie games have award-winning soundtracks?"
                    },
                    new Question{
                        QuestionId = 100,
                        QuizId = 6,
                        QuestionScore= 10,
                        Description = "Which of these soundtracks were recorded with live orchestras? (Select all correct answers)"
                    }, new Question{
                        QuestionId = 101,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which of the following are valid C# value types? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 102,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is the purpose of the 'using' statement?"
                    },
                    new Question{
                        QuestionId = 103,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which LINQ methods are used for projection? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 104,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is the output of 'int? x = null; int y = x ?? 5;'?"
                    },
                    new Question{
                        QuestionId = 105,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is the keyword to define an asynchronous method?"
                    },
                    new Question{
                        QuestionId = 106,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which are C# reference types? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 107,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which delegate types exist in .NET by default? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 108,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which keyword allows overriding a base class method?"
                    },
                    new Question{
                        QuestionId = 109,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is boxing in C#?"
                    },
                    new Question{
                        QuestionId = 110,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which C# features were introduced in C# 9.0? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 111,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which keywords are used in exception handling? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 112,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is the default access modifier for class members?"
                    },
                    new Question{
                        QuestionId = 113,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What interface does IDisposable imply?"
                    },
                    new Question{
                        QuestionId = 114,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which of the following types can be used with foreach? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 115,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What does the 'readonly' keyword mean?"
                    },
                    new Question{
                        QuestionId = 116,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which features are part of pattern matching in C#? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 117,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is true about 'Span<T>'?"
                    },
                    new Question{
                        QuestionId = 118,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which types support nullability in C# 8+? (Select all correct answers)"
                    },
                    new Question{
                        QuestionId = 119,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "Which constructs allow asynchronous streams?"
                    },
                    new Question{
                        QuestionId = 120,
                        QuizId = 7,
                        QuestionScore= 10,
                        Description = "What is the main advantage of using records?"
                    },
                };

                await context.Questions.AddRangeAsync(questions);
                await context.SaveChangesAsync();

                var answers = new List<Answer>
                {
                   new Answer{
                    AnswerId = 1,
                    QuestionId = 1,
                    Description = "Paris",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 2,
                    QuestionId = 1,
                    Description = "London",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 3,
                    QuestionId = 1,
                    Description = "Berlin",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 4,
                    QuestionId = 1,
                    Description = "Madrid",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 5,
                    QuestionId = 2,
                    Description = "3",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 6,
                    QuestionId = 2,
                    Description = "4",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 7,
                    QuestionId = 2,
                    Description = "5",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 8,
                    QuestionId = 3,
                    Description = "George Orwell",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 9,
                    QuestionId = 3,
                    Description = "Aldous Huxley",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 10,
                    QuestionId = 3,
                    Description = "J.K. Rowling",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 11,
                    QuestionId = 4,
                    Description = "100°C",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 12,
                    QuestionId = 4,
                    Description = "90°C",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 13,
                    QuestionId = 4,
                    Description = "120°C",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 14,
                    QuestionId = 4,
                    Description = "80°C",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 15,
                    QuestionId = 5,
                    Description = "Pacific",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 16,
                    QuestionId = 5,
                    Description = "Atlantic",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 17,
                    QuestionId = 5,
                    Description = "Indian",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 18,
                    QuestionId = 6,
                    Description = "Leonardo da Vinci",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 19,
                    QuestionId = 6,
                    Description = "Michelangelo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 20,
                    QuestionId = 6,
                    Description = "Van Gogh",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 21,
                    QuestionId = 7,
                    Description = "Yen",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 22,
                    QuestionId = 7,
                    Description = "Won",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 23,
                    QuestionId = 7,
                    Description = "Dollar",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 24,
                    QuestionId = 8,
                    Description = "Oxygen",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 25,
                    QuestionId = 8,
                    Description = "Hydrogen",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 26,
                    QuestionId = 8,
                    Description = "Carbon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 27,
                    QuestionId = 9,
                    Description = "Isaac Newton",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 28,
                    QuestionId = 9,
                    Description = "Albert Einstein",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 29,
                    QuestionId = 9,
                    Description = "Galileo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 30,
                    QuestionId = 10,
                    Description = "1945",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 31,
                    QuestionId = 10,
                    Description = "1939",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 32,
                    QuestionId = 10,
                    Description = "1950",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 33,
                    QuestionId = 11,
                    Description = "4",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 34,
                    QuestionId = 11,
                    Description = "8",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 35,
                    QuestionId = 11,
                    Description = "16",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 36,
                    QuestionId = 12,
                    Description = "Mars",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 37,
                    QuestionId = 12,
                    Description = "Jupiter",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 38,
                    QuestionId = 12,
                    Description = "Saturn",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 39,
                    QuestionId = 13,
                    Description = "Alexander Graham Bell",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 40,
                    QuestionId = 13,
                    Description = "Thomas Edison",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 41,
                    QuestionId = 13,
                    Description = "Nikola Tesla",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 42,
                    QuestionId = 14,
                    Description = "Portuguese",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 43,
                    QuestionId = 14,
                    Description = "Spanish",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 44,
                    QuestionId = 14,
                    Description = "French",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 45,
                    QuestionId = 15,
                    Description = "Carbon Dioxide",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 46,
                    QuestionId = 15,
                    Description = "Oxygen",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 47,
                    QuestionId = 15,
                    Description = "Nitrogen",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 48,
                    QuestionId = 16,
                    Description = "J.K. Rowling",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 49,
                    QuestionId = 16,
                    Description = "Stephen King",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 50,
                    QuestionId = 16,
                    Description = "George R.R. Martin",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 51,
                    QuestionId = 17,
                    Description = "Mount Everest",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 52,
                    QuestionId = 17,
                    Description = "K2",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 53,
                    QuestionId = 17,
                    Description = "Kilimanjaro",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 54,
                    QuestionId = 18,
                    Description = "Deoxyribonucleic Acid",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 55,
                    QuestionId = 18,
                    Description = "Ribonucleic Acid",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 56,
                    QuestionId = 18,
                    Description = "Nucleic Base",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 57,
                    QuestionId = 19,
                    Description = "Neil Armstrong",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 58,
                    QuestionId = 19,
                    Description = "Buzz Aldrin",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 59,
                    QuestionId = 19,
                    Description = "Yuri Gagarin",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 60,
                    QuestionId = 20,
                    Description = "299,792,458 m/s",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 61,
                    QuestionId = 20,
                    Description = "150,000,000 m/s",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 62,
                    QuestionId = 20,
                    Description = "1,000,000 m/s",
                    IsCorrect = false
                },new Answer{
                    AnswerId = 63,
                    QuestionId = 21,
                    Description = "Blind Guardian",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 64,
                    QuestionId = 21,
                    Description = "DragonForce",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 65,
                    QuestionId = 21,
                    Description = "Metallica",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 66,
                    QuestionId = 21,
                    Description = "Iron Maiden",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 67,
                    QuestionId = 22,
                    Description = "Sabaton",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 68,
                    QuestionId = 22,
                    Description = "Rhapsody of Fire",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 69,
                    QuestionId = 22,
                    Description = "Slipknot",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 70,
                    QuestionId = 22,
                    Description = "System of a Down",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 71,
                    QuestionId = 23,
                    Description = "Sonata Arctica",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 72,
                    QuestionId = 23,
                    Description = "Avantasia",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 73,
                    QuestionId = 23,
                    Description = "Kamelot",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 74,
                    QuestionId = 23,
                    Description = "Trivium",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 75,
                    QuestionId = 24,
                    Description = "Germany",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 76,
                    QuestionId = 24,
                    Description = "Sweden",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 77,
                    QuestionId = 24,
                    Description = "Finland",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 78,
                    QuestionId = 24,
                    Description = "Norway",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 79,
                    QuestionId = 25,
                    Description = "Thrash Metal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 80,
                    QuestionId = 25,
                    Description = "Power Metal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 81,
                    QuestionId = 25,
                    Description = "Death Metal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 82,
                    QuestionId = 25,
                    Description = "Black Metal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 83,
                    QuestionId = 26,
                    Description = "HammerFall",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 84,
                    QuestionId = 26,
                    Description = "Sabaton",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 85,
                    QuestionId = 26,
                    Description = "Blind Guardian",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 86,
                    QuestionId = 26,
                    Description = "Epica",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 87,
                    QuestionId = 27,
                    Description = "Kreator",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 88,
                    QuestionId = 27,
                    Description = "Rhapsody of Fire",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 89,
                    QuestionId = 27,
                    Description = "Nightwish",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 90,
                    QuestionId = 27,
                    Description = "Behemoth",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 91,
                    QuestionId = 28,
                    Description = "Kai Hansen",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 92,
                    QuestionId = 28,
                    Description = "James Hetfield",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 93,
                    QuestionId = 28,
                    Description = "Tony Iommi",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 94,
                    QuestionId = 28,
                    Description = "Bruce Dickinson",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 95,
                    QuestionId = 29,
                    Description = "DragonForce",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 96,
                    QuestionId = 29,
                    Description = "Iced Earth",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 97,
                    QuestionId = 29,
                    Description = "Gamma Ray",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 98,
                    QuestionId = 29,
                    Description = "HammerFall",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 99,
                    QuestionId = 30,
                    Description = "Keeper of the Seven Keys",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 100,
                    QuestionId = 30,
                    Description = "Black Album",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 101,
                    QuestionId = 30,
                    Description = "Follow the Leader",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 102,
                    QuestionId = 30,
                    Description = "Holy Diver",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 103,
                    QuestionId = 31,
                    Description = "Blind Guardian",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 104,
                    QuestionId = 31,
                    Description = "DragonForce",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 105,
                    QuestionId = 31,
                    Description = "Slayer",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 106,
                    QuestionId = 31,
                    Description = "Meshuggah",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 107,
                    QuestionId = 32,
                    Description = "Symphony of Enchanted Lands",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 108,
                    QuestionId = 32,
                    Description = "Eternal Glory",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 109,
                    QuestionId = 32,
                    Description = "Master of Puppets",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 110,
                    QuestionId = 32,
                    Description = "Dawn of Victory",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 111,
                    QuestionId = 33,
                    Description = "Double kick drums",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 112,
                    QuestionId = 33,
                    Description = "Synthesizers",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 113,
                    QuestionId = 33,
                    Description = "Turntables",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 114,
                    QuestionId = 33,
                    Description = "Electric guitar",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 115,
                    QuestionId = 34,
                    Description = "Fantasy",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 116,
                    QuestionId = 34,
                    Description = "War",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 117,
                    QuestionId = 34,
                    Description = "Street violence",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 118,
                    QuestionId = 34,
                    Description = "Science fiction",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 119,
                    QuestionId = 35,
                    Description = "Slipknot",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 120,
                    QuestionId = 35,
                    Description = "DragonForce",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 121,
                    QuestionId = 35,
                    Description = "Linkin Park",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 122,
                    QuestionId = 35,
                    Description = "Disturbed",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 123,
                    QuestionId = 36,
                    Description = "Kai Hansen",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 124,
                    QuestionId = 36,
                    Description = "Andi Deris",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 125,
                    QuestionId = 36,
                    Description = "Tuomas Holopainen",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 126,
                    QuestionId = 36,
                    Description = "Alexi Laiho",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 127,
                    QuestionId = 37,
                    Description = "Growling",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 128,
                    QuestionId = 37,
                    Description = "Clean high-pitched singing",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 129,
                    QuestionId = 37,
                    Description = "Screaming",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 130,
                    QuestionId = 37,
                    Description = "Spoken word",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 131,
                    QuestionId = 38,
                    Description = "Symphonic power metal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 132,
                    QuestionId = 38,
                    Description = "Black metal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 133,
                    QuestionId = 38,
                    Description = "Progressive power metal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 134,
                    QuestionId = 38,
                    Description = "Deathcore",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 135,
                    QuestionId = 39,
                    Description = "Fantasy creatures",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 136,
                    QuestionId = 39,
                    Description = "Abstract fractals",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 137,
                    QuestionId = 39,
                    Description = "Graffiti",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 138,
                    QuestionId = 39,
                    Description = "Street photography",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 139,
                    QuestionId = 40,
                    Description = "Nuclear Blast",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 140,
                    QuestionId = 40,
                    Description = "Island Records",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 141,
                    QuestionId = 40,
                    Description = "Death Row Records",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 142,
                    QuestionId = 40,
                    Description = "Cash Money Records",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 222,
                    QuestionId = 41,
                    Description = "Danube",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 143,
                    QuestionId = 41,
                    Description = "Volga",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 144,
                    QuestionId = 41,
                    Description = "Rhine",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 145,
                    QuestionId = 41,
                    Description = "Seine",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 146,
                    QuestionId = 42,
                    Description = "Switzerland",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 147,
                    QuestionId = 42,
                    Description = "Austria",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 148,
                    QuestionId = 42,
                    Description = "Portugal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 149,
                    QuestionId = 42,
                    Description = "Hungary",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 150,
                    QuestionId = 43,
                    Description = "Himalayas",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 151,
                    QuestionId = 43,
                    Description = "Andes",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 152,
                    QuestionId = 43,
                    Description = "Ural",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 153,
                    QuestionId = 43,
                    Description = "Alps",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 154,
                    QuestionId = 44,
                    Description = "Greece",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 155,
                    QuestionId = 44,
                    Description = "Sweden",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 156,
                    QuestionId = 44,
                    Description = "Italy",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 157,
                    QuestionId = 44,
                    Description = "Norway",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 158,
                    QuestionId = 45,
                    Description = "Monaco",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 159,
                    QuestionId = 45,
                    Description = "San Marino",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 160,
                    QuestionId = 45,
                    Description = "Vatican City",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 161,
                    QuestionId = 45,
                    Description = "Liechtenstein",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 162,
                    QuestionId = 46,
                    Description = "Bratislava",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 163,
                    QuestionId = 46,
                    Description = "Ljubljana",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 164,
                    QuestionId = 46,
                    Description = "Zagreb",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 165,
                    QuestionId = 46,
                    Description = "Budapest",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 166,
                    QuestionId = 47,
                    Description = "Poland",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 167,
                    QuestionId = 47,
                    Description = "Netherlands",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 168,
                    QuestionId = 47,
                    Description = "Spain",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 169,
                    QuestionId = 47,
                    Description = "Czech Republic",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 170,
                    QuestionId = 48,
                    Description = "Baltic Sea",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 171,
                    QuestionId = 48,
                    Description = "Adriatic Sea",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 172,
                    QuestionId = 48,
                    Description = "Black Sea",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 173,
                    QuestionId = 48,
                    Description = "North Sea",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 174,
                    QuestionId = 49,
                    Description = "Vienna",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 175,
                    QuestionId = 49,
                    Description = "Budapest",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 176,
                    QuestionId = 49,
                    Description = "Belgrade",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 177,
                    QuestionId = 49,
                    Description = "All of the above",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 178,
                    QuestionId = 50,
                    Description = "Germany",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 179,
                    QuestionId = 50,
                    Description = "Sweden",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 180,
                    QuestionId = 50,
                    Description = "France",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 181,
                    QuestionId = 50,
                    Description = "Poland",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 182,
                    QuestionId = 51,
                    Description = "Mont Blanc",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 183,
                    QuestionId = 51,
                    Description = "Matterhorn",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 184,
                    QuestionId = 51,
                    Description = "Elbrus",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 185,
                    QuestionId = 51,
                    Description = "Triglav",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 186,
                    QuestionId = 52,
                    Description = "Norway",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 187,
                    QuestionId = 52,
                    Description = "Denmark",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 188,
                    QuestionId = 52,
                    Description = "Finland",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 189,
                    QuestionId = 52,
                    Description = "Belgium",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 190,
                    QuestionId = 53,
                    Description = "Norway",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 191,
                    QuestionId = 53,
                    Description = "Iceland",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 192,
                    QuestionId = 53,
                    Description = "Italy",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 193,
                    QuestionId = 53,
                    Description = "Portugal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 194,
                    QuestionId = 54,
                    Description = "Vilnius",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 195,
                    QuestionId = 54,
                    Description = "Riga",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 196,
                    QuestionId = 54,
                    Description = "Tallinn",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 197,
                    QuestionId = 54,
                    Description = "Kaunas",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 198,
                    QuestionId = 55,
                    Description = "Lisbon",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 199,
                    QuestionId = 55,
                    Description = "Madrid",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 200,
                    QuestionId = 55,
                    Description = "London",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 201,
                    QuestionId = 55,
                    Description = "Oslo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 202,
                    QuestionId = 56,
                    Description = "Spain",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 203,
                    QuestionId = 56,
                    Description = "Portugal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 204,
                    QuestionId = 56,
                    Description = "France",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 205,
                    QuestionId = 56,
                    Description = "Andorra",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 206,
                    QuestionId = 57,
                    Description = "Thames",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 207,
                    QuestionId = 57,
                    Description = "Rhine",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 208,
                    QuestionId = 57,
                    Description = "Danube",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 209,
                    QuestionId = 57,
                    Description = "Seine",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 210,
                    QuestionId = 58,
                    Description = "Russia",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 211,
                    QuestionId = 58,
                    Description = "Turkey",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 212,
                    QuestionId = 58,
                    Description = "Georgia",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 213,
                    QuestionId = 58,
                    Description = "All of the above",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 214,
                    QuestionId = 59,
                    Description = "Zagreb",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 215,
                    QuestionId = 59,
                    Description = "Ljubljana",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 216,
                    QuestionId = 59,
                    Description = "Sarajevo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 217,
                    QuestionId = 59,
                    Description = "Skopje",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 218,
                    QuestionId = 60,
                    Description = "Belgium",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 219,
                    QuestionId = 60,
                    Description = "Netherlands",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 220,
                    QuestionId = 60,
                    Description = "France",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 221,
                    QuestionId = 60,
                    Description = "Germany",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 223,
                    QuestionId = 61,
                    Description = "Bulbasaur",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 224,
                    QuestionId = 61,
                    Description = "Charmander",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 225,
                    QuestionId = 61,
                    Description = "Pikachu",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 226,
                    QuestionId = 61,
                    Description = "Squirtle",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 227,
                    QuestionId = 62,
                    Description = "Water/Flying",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 228,
                    QuestionId = 62,
                    Description = "Water/Dragon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 229,
                    QuestionId = 62,
                    Description = "Flying/Dragon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 230,
                    QuestionId = 62,
                    Description = "Water/Ice",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 231,
                    QuestionId = 63,
                    Description = "Gold",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 232,
                    QuestionId = 63,
                    Description = "Silver",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 233,
                    QuestionId = 63,
                    Description = "Crystal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 234,
                    QuestionId = 63,
                    Description = "Ruby",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 235,
                    QuestionId = 64,
                    Description = "Pikachu",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 236,
                    QuestionId = 64,
                    Description = "Electabuzz",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 237,
                    QuestionId = 64,
                    Description = "Magnemite",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 238,
                    QuestionId = 64,
                    Description = "Machoke",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 239,
                    QuestionId = 65,
                    Description = "Sinnoh",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 240,
                    QuestionId = 65,
                    Description = "Kalos",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 241,
                    QuestionId = 65,
                    Description = "Unova",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 242,
                    QuestionId = 65,
                    Description = "Galar",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 243,
                    QuestionId = 66,
                    Description = "Flareon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 244,
                    QuestionId = 66,
                    Description = "Vaporeon",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 245,
                    QuestionId = 66,
                    Description = "Jolteon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 246,
                    QuestionId = 66,
                    Description = "Leafeon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 247,
                    QuestionId = 67,
                    Description = "Zapdos",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 248,
                    QuestionId = 67,
                    Description = "Articuno",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 249,
                    QuestionId = 67,
                    Description = "Mewtwo",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 250,
                    QuestionId = 67,
                    Description = "Dragonite",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 251,
                    QuestionId = 68,
                    Description = "Catch all Pokémon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 252,
                    QuestionId = 68,
                    Description = "Beat the Elite Four",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 253,
                    QuestionId = 68,
                    Description = "Collect gym badges",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 254,
                    QuestionId = 68,
                    Description = "All of the above",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 255,
                    QuestionId = 69,
                    Description = "Nidorina",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 256,
                    QuestionId = 69,
                    Description = "Clefairy",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 257,
                    QuestionId = 69,
                    Description = "Jigglypuff",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 258,
                    QuestionId = 69,
                    Description = "Pidgeotto",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 259,
                    QuestionId = 70,
                    Description = "Dark",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 260,
                    QuestionId = 70,
                    Description = "Fairy",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 261,
                    QuestionId = 70,
                    Description = "Steel",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 262,
                    QuestionId = 70,
                    Description = "Normal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 263,
                    QuestionId = 71,
                    Description = "Ruby",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 264,
                    QuestionId = 71,
                    Description = "Sapphire",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 265,
                    QuestionId = 71,
                    Description = "Emerald",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 266,
                    QuestionId = 71,
                    Description = "Black",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 267,
                    QuestionId = 72,
                    Description = "Magikarp",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 268,
                    QuestionId = 72,
                    Description = "Gyarados",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 269,
                    QuestionId = 72,
                    Description = "Wobbuffet",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 270,
                    QuestionId = 72,
                    Description = "Psyduck",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 271,
                    QuestionId = 73,
                    Description = "Lugia",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 272,
                    QuestionId = 73,
                    Description = "Dialga",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 273,
                    QuestionId = 73,
                    Description = "Zacian",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 274,
                    QuestionId = 73,
                    Description = "Ditto",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 275,
                    QuestionId = 74,
                    Description = "Sylveon",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 276,
                    QuestionId = 74,
                    Description = "Umbreon",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 277,
                    QuestionId = 74,
                    Description = "Glaceon",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 278,
                    QuestionId = 74,
                    Description = "Pikachu",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 279,
                    QuestionId = 75,
                    Description = "Charizard",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 280,
                    QuestionId = 75,
                    Description = "Gengar",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 281,
                    QuestionId = 75,
                    Description = "Onix",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 282,
                    QuestionId = 75,
                    Description = "Alakazam",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 283,
                    QuestionId = 76,
                    Description = "Snorlax",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 284,
                    QuestionId = 76,
                    Description = "Charizard",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 285,
                    QuestionId = 76,
                    Description = "Eevee",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 286,
                    QuestionId = 76,
                    Description = "Arceus",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 287,
                    QuestionId = 77,
                    Description = "Fighting",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 288,
                    QuestionId = 77,
                    Description = "Steel",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 289,
                    QuestionId = 77,
                    Description = "Fighting/Steel",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 290,
                    QuestionId = 77,
                    Description = "Psychic",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 291,
                    QuestionId = 78,
                    Description = "Brock",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 292,
                    QuestionId = 78,
                    Description = "Misty",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 293,
                    QuestionId = 78,
                    Description = "Erika",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 294,
                    QuestionId = 78,
                    Description = "Jessie",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 295,
                    QuestionId = 79,
                    Description = "Omanyte",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 296,
                    QuestionId = 79,
                    Description = "Kabuto",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 297,
                    QuestionId = 79,
                    Description = "Aerodactyl",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 298,
                    QuestionId = 79,
                    Description = "Farfetch’d",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 299,
                    QuestionId = 80,
                    Description = "Lugia",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 300,
                    QuestionId = 80,
                    Description = "Ho-Oh",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 301,
                    QuestionId = 80,
                    Description = "Raikou",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 302,
                    QuestionId = 80,
                    Description = "Togepi",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 303,
                    QuestionId = 81,
                    Description = "Final Fantasy VII",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 304,
                    QuestionId = 81,
                    Description = "Final Fantasy X",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 305,
                    QuestionId = 81,
                    Description = "Chrono Trigger",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 306,
                    QuestionId = 81,
                    Description = "The Legend of Zelda",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 307,
                    QuestionId = 82,
                    Description = "Jeremy Soule",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 308,
                    QuestionId = 82,
                    Description = "Koji Kondo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 309,
                    QuestionId = 82,
                    Description = "Austin Wintory",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 310,
                    QuestionId = 82,
                    Description = "Yoko Shimomura",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 311,
                    QuestionId = 83,
                    Description = "Halo",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 312,
                    QuestionId = 83,
                    Description = "Final Fantasy",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 313,
                    QuestionId = 83,
                    Description = "God of War",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 314,
                    QuestionId = 83,
                    Description = "Minecraft",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 315,
                    QuestionId = 84,
                    Description = "Metroid",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 316,
                    QuestionId = 84,
                    Description = "The Legend of Zelda",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 317,
                    QuestionId = 84,
                    Description = "Dark Souls",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 318,
                    QuestionId = 84,
                    Description = "Kingdom Hearts",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 319,
                    QuestionId = 85,
                    Description = "Ambient",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 320,
                    QuestionId = 85,
                    Description = "Jazz",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 321,
                    QuestionId = 85,
                    Description = "Heavy Metal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 322,
                    QuestionId = 85,
                    Description = "Chiptune",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 323,
                    QuestionId = 86,
                    Description = "Yoko Shimomura",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 324,
                    QuestionId = 86,
                    Description = "Nobuo Uematsu",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 325,
                    QuestionId = 86,
                    Description = "Koji Kondo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 326,
                    QuestionId = 86,
                    Description = "Grant Kirkhope",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 327,
                    QuestionId = 87,
                    Description = "Final Fantasy IX",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 328,
                    QuestionId = 87,
                    Description = "Final Fantasy X",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 329,
                    QuestionId = 87,
                    Description = "Persona 5",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 330,
                    QuestionId = 87,
                    Description = "NieR: Automata",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 331,
                    QuestionId = 88,
                    Description = "Keiichi Okabe",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 332,
                    QuestionId = 88,
                    Description = "Emi Evans",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 333,
                    QuestionId = 88,
                    Description = "Shoji Meguro",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 334,
                    QuestionId = 88,
                    Description = "Mick Gordon",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 335,
                    QuestionId = 89,
                    Description = "Civilization IV",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 336,
                    QuestionId = 89,
                    Description = "Age of Empires",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 337,
                    QuestionId = 89,
                    Description = "Spore",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 338,
                    QuestionId = 89,
                    Description = "Black & White",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 339,
                    QuestionId = 90,
                    Description = "Smash Bros.",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 340,
                    QuestionId = 90,
                    Description = "Fire Emblem",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 341,
                    QuestionId = 90,
                    Description = "Xenoblade Chronicles",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 342,
                    QuestionId = 90,
                    Description = "Bayonetta",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 343,
                    QuestionId = 91,
                    Description = "DOOM Eternal",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 344,
                    QuestionId = 91,
                    Description = "Red Dead Redemption 2",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 345,
                    QuestionId = 91,
                    Description = "The Witcher 3",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 346,
                    QuestionId = 91,
                    Description = "Celeste",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 347,
                    QuestionId = 92,
                    Description = "Super Mario Bros.",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 348,
                    QuestionId = 92,
                    Description = "The Legend of Zelda",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 349,
                    QuestionId = 92,
                    Description = "Donkey Kong Country",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 350,
                    QuestionId = 92,
                    Description = "Ocarina of Time",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 351,
                    QuestionId = 93,
                    Description = "Undertale",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 352,
                    QuestionId = 93,
                    Description = "Hollow Knight",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 353,
                    QuestionId = 93,
                    Description = "Cuphead",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 354,
                    QuestionId = 93,
                    Description = "Celeste",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 355,
                    QuestionId = 94,
                    Description = "Harry Gregson-Williams",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 356,
                    QuestionId = 94,
                    Description = "Nobuo Uematsu",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 357,
                    QuestionId = 94,
                    Description = "Koji Kondo",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 358,
                    QuestionId = 94,
                    Description = "Akira Yamaoka",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 359,
                    QuestionId = 95,
                    Description = "Dark Souls",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 360,
                    QuestionId = 95,
                    Description = "Undertale",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 361,
                    QuestionId = 95,
                    Description = "NieR: Automata",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 362,
                    QuestionId = 95,
                    Description = "The Sims",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 363,
                    QuestionId = 96,
                    Description = "Shadow of the Colossus",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 364,
                    QuestionId = 96,
                    Description = "Dark Souls",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 365,
                    QuestionId = 96,
                    Description = "Skyrim",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 366,
                    QuestionId = 96,
                    Description = "God of War",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 367,
                    QuestionId = 97,
                    Description = "Austin Wintory",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 368,
                    QuestionId = 97,
                    Description = "Jesper Kyd",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 369,
                    QuestionId = 97,
                    Description = "Gareth Coker",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 370,
                    QuestionId = 97,
                    Description = "Yuzo Koshiro",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 371,
                    QuestionId = 98,
                    Description = "Journey",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 372,
                    QuestionId = 98,
                    Description = "God of War (2018)",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 373,
                    QuestionId = 98,
                    Description = "NieR: Automata",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 374,
                    QuestionId = 98,
                    Description = "Cyberpunk 2077",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 375,
                    QuestionId = 99,
                    Description = "Celeste",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 376,
                    QuestionId = 99,
                    Description = "Hades",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 377,
                    QuestionId = 99,
                    Description = "Hollow Knight",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 378,
                    QuestionId = 99,
                    Description = "Among Us",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 379,
                    QuestionId = 100,
                    Description = "Final Fantasy VII Remake",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 380,
                    QuestionId = 100,
                    Description = "The Last of Us Part II",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 381,
                    QuestionId = 100,
                    Description = "Zelda: Breath of the Wild",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 382,
                    QuestionId = 100,
                    Description = "Undertale",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 383,
                    QuestionId = 101,
                    Description = "int",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 384,
                    QuestionId = 101,
                    Description = "string",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 385,
                    QuestionId = 101,
                    Description = "bool",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 386,
                    QuestionId = 101,
                    Description = "struct",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 387,
                    QuestionId = 102,
                    Description = "To include namespaces",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 388,
                    QuestionId = 102,
                    Description = "To manage IDisposable objects",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 389,
                    QuestionId = 102,
                    Description = "To define access scope",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 390,
                    QuestionId = 102,
                    Description = "To create generic types",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 391,
                    QuestionId = 103,
                    Description = "Select",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 392,
                    QuestionId = 103,
                    Description = "Where",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 393,
                    QuestionId = 103,
                    Description = "GroupBy",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 394,
                    QuestionId = 103,
                    Description = "SelectMany",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 395,
                    QuestionId = 104,
                    Description = "null",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 396,
                    QuestionId = 104,
                    Description = "5",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 397,
                    QuestionId = 104,
                    Description = "0",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 398,
                    QuestionId = 104,
                    Description = "Compiler error",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 399,
                    QuestionId = 105,
                    Description = "async",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 400,
                    QuestionId = 105,
                    Description = "await",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 401,
                    QuestionId = 105,
                    Description = "yield",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 402,
                    QuestionId = 105,
                    Description = "defer",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 403,
                    QuestionId = 106,
                    Description = "string",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 404,
                    QuestionId = 106,
                    Description = "object",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 405,
                    QuestionId = 106,
                    Description = "class",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 406,
                    QuestionId = 106,
                    Description = "int",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 407,
                    QuestionId = 107,
                    Description = "Action",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 408,
                    QuestionId = 107,
                    Description = "Func",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 409,
                    QuestionId = 107,
                    Description = "Predicate",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 410,
                    QuestionId = 107,
                    Description = "Listener",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 411,
                    QuestionId = 108,
                    Description = "override",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 412,
                    QuestionId = 108,
                    Description = "virtual",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 413,
                    QuestionId = 108,
                    Description = "sealed",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 414,
                    QuestionId = 108,
                    Description = "abstract",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 415,
                    QuestionId = 109,
                    Description = "Converting value type to object",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 416,
                    QuestionId = 109,
                    Description = "Converting object to value type",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 417,
                    QuestionId = 109,
                    Description = "Creating threads",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 418,
                    QuestionId = 109,
                    Description = "Initializing arrays",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 419,
                    QuestionId = 110,
                    Description = "Records",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 420,
                    QuestionId = 110,
                    Description = "Init-only setters",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 421,
                    QuestionId = 110,
                    Description = "Pattern matching",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 422,
                    QuestionId = 110,
                    Description = "Dynamic",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 423,
                    QuestionId = 111,
                    Description = "try",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 424,
                    QuestionId = 111,
                    Description = "catch",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 425,
                    QuestionId = 111,
                    Description = "finally",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 426,
                    QuestionId = 111,
                    Description = "throw",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 427,
                    QuestionId = 112,
                    Description = "private",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 428,
                    QuestionId = 112,
                    Description = "public",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 429,
                    QuestionId = 112,
                    Description = "protected",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 430,
                    QuestionId = 112,
                    Description = "internal",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 431,
                    QuestionId = 113,
                    Description = "The object has a Dispose() method",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 432,
                    QuestionId = 113,
                    Description = "The object can be reset",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 433,
                    QuestionId = 113,
                    Description = "The object is enumerable",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 434,
                    QuestionId = 113,
                    Description = "The object is comparable",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 435,
                    QuestionId = 114,
                    Description = "Array",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 436,
                    QuestionId = 114,
                    Description = "List",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 437,
                    QuestionId = 114,
                    Description = "Dictionary",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 438,
                    QuestionId = 114,
                    Description = "int",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 439,
                    QuestionId = 115,
                    Description = "Field can only be assigned in constructor",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 440,
                    QuestionId = 115,
                    Description = "Variable cannot change at runtime",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 441,
                    QuestionId = 115,
                    Description = "It's a constant",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 442,
                    QuestionId = 115,
                    Description = "Used for static variables",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 443,
                    QuestionId = 116,
                    Description = "switch expressions",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 444,
                    QuestionId = 116,
                    Description = "type patterns",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 445,
                    QuestionId = 116,
                    Description = "property patterns",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 446,
                    QuestionId = 116,
                    Description = "method patterns",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 447,
                    QuestionId = 117,
                    Description = "It's allocated on the stack",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 448,
                    QuestionId = 117,
                    Description = "Used for high-performance memory access",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 449,
                    QuestionId = 117,
                    Description = "Requires garbage collection",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 450,
                    QuestionId = 117,
                    Description = "Unsafe by default",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 451,
                    QuestionId = 118,
                    Description = "string?",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 452,
                    QuestionId = 118,
                    Description = "int?",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 453,
                    QuestionId = 118,
                    Description = "List<string>?",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 454,
                    QuestionId = 118,
                    Description = "bool?",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 455,
                    QuestionId = 119,
                    Description = "IAsyncEnumerable<T>",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 456,
                    QuestionId = 119,
                    Description = "yield return",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 457,
                    QuestionId = 119,
                    Description = "await foreach",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 458,
                    QuestionId = 119,
                    Description = "Func<T>",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 459,
                    QuestionId = 120,
                    Description = "Value-based equality",
                    IsCorrect = true
                },
                new Answer{
                    AnswerId = 460,
                    QuestionId = 120,
                    Description = "Better serialization",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 461,
                    QuestionId = 120,
                    Description = "Inheritance safety",
                    IsCorrect = false
                },
                new Answer{
                    AnswerId = 462,
                    QuestionId = 120,
                    Description = "Runtime performance",
                    IsCorrect = false
                },
            };

                await context.Answers.AddRangeAsync(answers);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                Console.WriteLine("The database has been seeded successfully!");
            }
            catch (System.Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"An error during seeding: {ex.Message}");
            }
        }

    }
}