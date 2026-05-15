using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using quiz_project.RuntimeModels;
using static quiz_project.RuntimeModels.QuizMetaData;


namespace quiz_project.Extensions
{
    public static class SessionExtensions
    {
        public static void SetInt(this ISession session, SessionKeys key, int value)
            => session.SetInt32(key.ToString(), value);

        public static int? GetInt(this ISession session, SessionKeys key)
            => session.GetInt32(key.ToString());

        public static void SetString(this ISession session, SessionKeys key, string value)
            => session.SetString(key.ToString(), value);

        public static string? GetString(this ISession session, SessionKeys key)
            => session.GetString(key.ToString());
        public static void SetQuizMetaDataJSON(this ISession session, SessionKeys key, string json)
        {
            try
            {
                JsonSerializer.Serialize(json);
                session.SetString(key.ToString(), json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong: {e}");
            }
        }
        public static QuizMetaData GetQuizMetaDataJSON(this ISession session, SessionKeys key)
        {
            try
            {
                var jsonString = session.GetString(key.ToString());
                var json = JsonSerializer.Deserialize<QuizMetaData>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return json;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong: {e}");
            }
            return new QuizMetaData(-1, -1, -1, -1, new List<AnswersForQuestion>());
        }

        public static void Remove(this ISession session, SessionKeys key)
            => session.Remove(key.ToString());


    }
}