using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBlogAPI.Class;

namespace WebBlogAPI.Controllers
{
    internal class MySQLConnector
    {
        MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder
        {
            Server = "localhost",//host.docker.internal
            UserID = "apiuser",
            Password = "chikorita",
            Database = "lucifabdev"
        };

        public async Task<int> PushPostAsync(Class.Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));

            using (var connection = new MySqlConnection(connectionString.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO posts (title,content,author_id) VALUES (@title, @content, @authorid);";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@title", post.Title);
                    command.Parameters.AddWithValue("@content", post.Content);
                    command.Parameters.AddWithValue("@authorid", post.AuthorId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return 0;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            List<Post> posts = new List<Post>();

            using (var connection = new MySqlConnection(connectionString.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT posts.id,posts.title,posts.content,posts.created_at,posts.updated_at,a.name,a.name as AuthorName,posts.author_id FROM posts " +
                    "INNER JOIN authors a on posts.author_id=a.id";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            posts.Add(new Post
                            {
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Content = reader.GetString("content"),
                                CreatedAt = reader.GetDateTime("created_at"),
                                UpdatedAt = reader.GetDateTime("updated_at"),
                                AuthorName = reader.GetString("AuthorName"),
                                AuthorId = reader.GetInt32("author_id")
                            });
                        }
                    }
                }
            }
            return posts;
        }
        
    }
}
