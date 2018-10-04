using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ContestPark.Infrastructure.Category
{
    public class Post
    {
        [Required]
        public string ID { get; set; } = DateTime.UtcNow.Ticks.ToString();

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public bool IsPublished { get; set; } = true;

        public IList<string> Categories { get; set; } = new List<string>();

        public IList<Comment> Comments { get; } = new List<Comment>();

        public string GetLink()
        {
            return $"/blog/{Slug}/";
        }

        public bool AreCommentsOpen(int commentsCloseAfterDays)
        {
            return PubDate.AddDays(commentsCloseAfterDays) >= DateTime.UtcNow;
        }

        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = RemoveReservedUrlCharacters(title);

            return title.ToLowerInvariant();
        }

        private static string RemoveReservedUrlCharacters(string text)
        {
            var reservedCharacters = new List<string>() { "!", "#", "$", "&", "'", "(", ")", "*", ",", "/", ":", ";", "=", "?", "@", "[", "]", "\"", "%", ".", "<", ">", "\\", "^", "_", "'", "{", "}", "|", "~", "`", "+" };

            foreach (var chr in reservedCharacters)
            {
                text = text.Replace(chr, "");
            }

            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public string RenderContent()
        {
            var result = Content;

            // Set up lazy loading of images/iframes
            result = result.Replace(" src=\"", " src=\"data:image/gif;base64,R0lGODlhAQABAIAAAP///wAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw==\" data-src=\"");

            // Youtube content embedded using this syntax: [youtube:xyzAbc123]
            var video = "<div class=\"video\"><iframe width=\"560\" height=\"315\" title=\"YouTube embed\" src=\"about:blank\" data-src=\"https://www.youtube-nocookie.com/embed/{0}?modestbranding=1&amp;hd=1&amp;rel=0&amp;theme=light\" allowfullscreen></iframe></div>";
            result = Regex.Replace(result.ToString(), @"\[youtube:(.*?)\]", (Match m) => string.Format(video, m.Groups[1].Value));

            return result.ToString();
        }
    }

    public class Comment
    {
        [Required]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Author { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PubDate { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; }

        public string GetGravatar()
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(Email.Trim().ToLowerInvariant());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return $"https://www.gravatar.com/avatar/{sb.ToString().ToLowerInvariant()}?s=60&d=blank";
            }
        }

        public string RenderContent()
        {
            return Content;
        }
    }

    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(
            this IServiceCollection services)
        {
            var url = Environment.GetEnvironmentVariable("ElasticSearchURI");
            var defaultIndex = "miniblog";

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<Post>(m => m
                    .Ignore(p => p.IsPublished)
                    .PropertyName(p => p.ID, "id")
                )
                .DefaultMappingFor<Comment>(m => m
                    .Ignore(c => c.Email)
                    .Ignore(c => c.IsAdmin)
                    .PropertyName(c => c.ID, "id")
                );
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.CreateIndex(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .CharFilters(cf => cf
                            .Mapping("programming_language", mcf => mcf
                                .Mappings(
                                    "c# => csharp",
                                    "C# => Csharp"
                                )
                            )
                        )
                        .Analyzers(an => an
                            .Custom("content", ca => ca
                                .CharFilters("html_strip", "programming_language")
                                .Tokenizer("standard")
                                .Filters("standard", "lowercase", "stop")
                            )
                            .Custom("categories", ca => ca
                                .CharFilters("programming_language")
                                .Tokenizer("standard")
                                .Filters("standard", "lowercase")
                            )
                        )
                    )
                )
                .Mappings(m => m
                    .Map<Post>(x => x
                        .AutoMap()
                        .Properties(p => p
                            .Text(t => t
                                .Name(n => n.Title)
                                .Boost(3)
                            )
                            .Text(t => t
                                .Name(n => n.Content)
                                .Analyzer("content")
                                .Boost(1)
                            )
                            .Text(t => t
                                .Name(n => n.Excerpt)
                                .Boost(2)
                            )
                            .Text(t => t
                                .Name(n => n.Categories)
                                .Analyzer("categories")
                                .Boost(2)
                            )
                            .Nested<Comment>(np => np
                                .AutoMap()
                                .Name(nn => nn.Comments)
                                .Properties(cp => cp
                                    .Text(t => t
                                        .Name(n => n.Author)
                                        .Boost(0.6)
                                    )
                                    .Text(t => t
                                        .Name(n => n.Content)
                                        .Boost(0.5)
                                    )
                                )
                            )
                        )
                    )
                )
            );
        }
    }
}