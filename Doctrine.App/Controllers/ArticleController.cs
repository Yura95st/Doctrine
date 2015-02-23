namespace Doctrine.App.Controllers
{
    using System;
    using System.Web.Mvc;

    using Doctrine.App.ViewModels;
    using Doctrine.Domain.Models;

    public class ArticleController : Controller
    {
        private readonly ArticleViewModel _article;

        public ArticleController()
        {
            this._article = new ArticleViewModel
            {
                Article =
                    new Article
                    {
                        ArticleId = 1, PublicationDate = new DateTime(2015, 2, 22, 17, 24, 45),
                        Title = "Делаем приватный монитор из старого LCD монитора",
                        Text =
                            @"<br>Вы наконец-то можете сделать кое-что со своим старым LCD монитором, который завалялся у Вас в гараже. Превратите его в шпионский монитор! Для всех вокруг он будет выглядеть просто белым экраном, но не для Вас, потому что у Вас будут специальные «волшебные» очки.<br><br>Всё что Вам нужно – это пара старых очков, нож для бумаги и растворитель для краски.<br><br>",
                        Topic = new Topic { TopicId = 2, Name = "DIY или Сделай Сам" },
                        Tags =
                        {
                            new Tag { TagId = 3, Name = "tag one" }, new Tag { TagId = 4, Name = "tag two" },
                            new Tag { TagId = 5, Name = "tag three" }, new Tag { TagId = 6, Name = "tag four" }
                        },
                        User = new User { UserId = 7, FirstName = "John", LastName = "Grey" }
                    },
                ViewsCount = 137,
                CommentsCount = 6, FavsCount = 12, PositiveVotesCount = 1234, NegativeVotesCount = 234,
                Comments =
                    new[]
                    {
                        new CommentViewModel
                        {
                            Comment =
                                new Comment
                                {
                                    CommentId = 8, Date = new DateTime(2015, 2, 22, 17, 24, 45),
                                    Text = "Some comment text.<br> Comment line.",
                                    CommentEdit = new CommentEdit { EditDate = new DateTime(2015, 2, 22, 17, 26, 07) },
                                    User = new User { UserId = 7, FirstName = "John", LastName = "Grey" }
                                },
                                PositiveVotesCount = 1234, NegativeVotesCount = 234,
                                IsFromArticleAuthor = true
                        },
                        new CommentViewModel
                        {
                            Comment =
                                new Comment
                                {
                                    CommentId = 9, Date = new DateTime(2015, 2, 22, 17, 24, 45),
                                    Text = "Some comment text.<br> Comment line.",
                                    User = new User { UserId = 7, FirstName = "John", LastName = "Grey" }
                                },
                            Replies =
                                new[]
                                {
                                    new CommentViewModel
                                    {
                                        Comment =
                                            new Comment
                                            {
                                                CommentId = 10, Date = new DateTime(2015, 2, 22, 17, 24, 45),
                                                Text = "Some comment text.<br> Comment line.",
                                                User = new User { UserId = 7, FirstName = "John", LastName = "Grey" }
                                            },
                                        Replies =
                                            new[]
                                            {
                                                new CommentViewModel
                                                {
                                                    Comment =
                                                        new Comment
                                                        {
                                                            CommentId = 11, Date = new DateTime(2015, 2, 22, 17, 24, 45),
                                                            Text = "Some comment text.<br> Comment line.",
                                                            User =
                                                                new User
                                                                { UserId = 7, FirstName = "John", LastName = "Grey" }
                                                        },
                                                    Replies =
                                                        new[]
                                                        {
                                                            new CommentViewModel
                                                            {
                                                                Comment =
                                                                    new Comment
                                                                    {
                                                                        CommentId = 12,
                                                                        Date = new DateTime(2015, 2, 22, 17, 24, 45),
                                                                        Text = "Some comment text.<br> Comment line.",
                                                                        User =
                                                                            new User
                                                                            {
                                                                                UserId = 7, FirstName = "John",
                                                                                LastName = "Grey"
                                                                            }
                                                                    }
                                                            }
                                                        }
                                                }
                                            }
                                    }
                                }
                        },
                        new CommentViewModel
                        {
                            Comment =
                                new Comment
                                {
                                    CommentId = 13, Date = new DateTime(2015, 2, 22, 17, 24, 45),
                                    Text = "Some comment text.<br> Comment line.",
                                    User = new User { UserId = 7, FirstName = "John", LastName = "Grey" }
                                }
                        }
                    }
            };
        }

        public ActionResult Index()
        {
            this._article.IsPreviewMode = true;

            return this.View(this._article);
        }

        public ActionResult Read()
        {
            return this.View(this._article);
        }
    }
}