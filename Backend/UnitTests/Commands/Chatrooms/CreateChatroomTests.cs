﻿// using System.Security.Claims;
// using BusinessLogic.Commands.Chatrooms.CreateChatroom;
// using BusinessLogic.Services.UsersService;
// using Database;
// using Entities;
// using Entities.Chatrooms;
// using Extensions;
// using Microsoft.AspNetCore.Http;
// using Moq;
//
// namespace UnitTests.Commands.Chatrooms;
//
// public class CreateChatroomTests
// {
//     private readonly Mock<IStorageService> _storageServiceMock = new();
//     private readonly Mock<IUsersService> _usersServiceMock = new();
//
//     [Theory]
//     [InlineData(1)]
//     [InlineData(2)]
//     [InlineData(3)]
//     [InlineData(100)]
//     public async void CreateChatroom_CreatesPublicChatroom(int usersCount)
//     {
//         var users = CreateNUsers(usersCount).ToList();
//         _usersServiceMock.Setup(u => u.GetCurrentUser(Any<CancellationToken>(), Any<IQueryOptions<User>>()))
//                          .Returns(Task.FromResult(users.First())!);
//         _storageServiceMock.Setup(s => s.GetUsers(Any<Func<User, bool>>(), Any<CancellationToken>(), Any<int>(),
//                                Any<int>(), Any<IQueryOptions<User>>()))
//                            .Returns(users.AsAsyncEnumerable());
//         Chatroom created = null!;
//         _storageServiceMock.Setup(s => s.AddChatroomAsync(Any<Chatroom>(), Any<CancellationToken>()))
//                            .Callback<Chatroom, CancellationToken>((chat, _) => created = chat);
//         _storageServiceMock.Setup(s => s.SaveChangesAsync(Any<CancellationToken>()))
//                            .Returns(Task.CompletedTask);
//
//         var handler = new CreateChatroomHandler(
//             _storageServiceMock.Object,
//             _usersServiceMock.Object);
//
//         const string chatName = "some name";
//         var request = new CreateChatroomCommand
//                           {
//                               Name = chatName,
//                               Type = ChatType.Public,
//                               Usernames = users.Select(u => u.Username).ToList()
//                           };
//         var result = await handler.Handle(request, CancellationToken.None);
//         Assert.True(result.Created);
//         Assert.NotEqual(result.ChatId, Guid.Empty);
//         Assert.NotNull(created);
//         Assert.True(ListExtensions.EqualAsSets(created.Users.ToList(), users));
//         Assert.Equal(ChatType.Public, created.Type);
//         Assert.True(created is PublicChatroom);
//         var c = created as PublicChatroom;
//         Assert.Equal(chatName, c!.Name);
//         Assert.Equal(c.Administrators.Owner, users.First());
//     }
//
//     [Theory]
//     [InlineData(null)]
//     [InlineData("123")]
//     public async void CreateChatroom_CreatesPrivateChatroom(string? name)
//     {
//         var users = CreateNUsers(2).ToList();
//         _usersServiceMock.Setup(u => u.GetCurrentUser(Any<CancellationToken>(), Any<IQueryOptions<User>>()))
//                          .Returns(Task.FromResult(users.First())!);
//         _storageServiceMock.Setup(s => s.GetUsers(Any<Func<User, bool>>(), Any<CancellationToken>(), Any<int>(),
//             Any<int>(), Any<IQueryOptions<User>>())).Returns(users.AsAsyncEnumerable());
//         _storageServiceMock.Setup(s => s.GetChatrooms(Any<Func<Chatroom, bool>>(), Any<CancellationToken>(), Any<int>(),
//                                Any<int>(), Any<IQueryOptions<Chatroom>>()))
//                            .Returns(AsyncEnumerable.Empty<Chatroom>());
//         Chatroom created = null!;
//         _storageServiceMock.Setup(s => s.AddChatroomAsync(Any<Chatroom>(), Any<CancellationToken>()))
//                            .Callback<Chatroom, CancellationToken>((chat, _) => created = chat);
//         _storageServiceMock.Setup(s => s.SaveChangesAsync(Any<CancellationToken>()))
//                            .Returns(Task.CompletedTask);
//         var handler = new CreateChatroomHandler(
//             _storageServiceMock.Object,
//             _usersServiceMock.Object);
//
//         var request = new CreateChatroomCommand
//                           {
//                               Name = name, Type = ChatType.Private, Usernames = users.Select(u => u.Username).ToList()
//                           };
//         var result = await handler.Handle(request, CancellationToken.None);
//         Assert.True(result.Created);
//         Assert.NotEqual(result.ChatId, Guid.Empty);
//         Assert.True(ListExtensions.EqualAsSets(created.Users.ToList(), users));
//         Assert.Equal(ChatType.Private, created.Type);
//         Assert.True(created is PrivateChatroom);
//     }
//
//     [Theory]
//     [InlineData(ChatType.Private)]
//     [InlineData(ChatType.Public)]
//     public async void CreateChatroom_WithNoUsers_DoesntCreateChatroom(ChatType type)
//     {
//         var request = new CreateChatroomCommand
//                           {
//                               Name = "123", Type = type, Usernames = Enumerable.Empty<string>().ToList()
//                           };
//         var handler = new CreateChatroomHandler(
//             _storageServiceMock.Object,
//             _usersServiceMock.Object);
//
//         var result = await handler.Handle(request, CancellationToken.None);
//         Assert.False(result.Created);
//         Assert.Equal(result.ChatId, Guid.Empty);
//         _storageServiceMock.Verify(s => s.AddChatroomAsync(Any<Chatroom>(), Any<CancellationToken>()),
//             Times.Never());
//     }
//
//     [Fact]
//     public async void CreateChatroom_DuplicatePrivateChatroom_ChatroomIsNotCreated()
//     {
//         var users = CreateNUsers(2).ToList();
//         var chatroom = new PrivateChatroom(Guid.NewGuid(), users);
//         users.ForEach(u => u.Join(chatroom));
//
//         _usersServiceMock.Setup(u => u.GetCurrentUser(Any<CancellationToken>(), Any<IQueryOptions<User>>()))
//                          .Returns(Task.FromResult(users.First())!);
//         _storageServiceMock.Setup(s => s.GetUsers(Any<Func<User, bool>>(), Any<CancellationToken>(), Any<int>(),
//             Any<int>(), Any<IQueryOptions<User>>())).Returns(users.AsAsyncEnumerable());
//         _storageServiceMock.Setup(s => s.GetChatrooms(Any<Func<Chatroom, bool>>(), Any<CancellationToken>(), Any<int>(),
//                                Any<int>(), Any<IQueryOptions<Chatroom>>()))
//                            .Returns(AsyncEnumerable.Of(chatroom));
//
//         var request = new CreateChatroomCommand
//                           {
//                               Usernames = users.Select(u => u.Username).ToList(), Type = ChatType.Private,
//                           };
//
//         var handler = new CreateChatroomHandler(
//             _storageServiceMock.Object,
//             _usersServiceMock.Object);
//
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         Assert.False(result.Created);
//         Assert.Equal(result.ChatId, Guid.Empty);
//         _storageServiceMock.Verify(s => s.AddChatroomAsync(Any<Chatroom>(), Any<CancellationToken>()),
//             Times.Never());
//     }
//
//     [Theory]
//     [InlineData(1)]
//     [InlineData(3)]
//     [InlineData(5)]
//     public async void CreateChatroom_PrivateChatroomWithNot2Users_DoesntCreateChatroom(int usersCount)
//     {
//         var users = CreateNUsers(usersCount).ToList();
//
//         _usersServiceMock.Setup(u => u.GetCurrentUser(Any<CancellationToken>(), Any<IQueryOptions<User>>()))
//                          .Returns(Task.FromResult(users.First())!);
//         _storageServiceMock.Setup(s => s.GetUsers(Any<Func<User, bool>>(), Any<CancellationToken>(), Any<int>(),
//             Any<int>(), Any<IQueryOptions<User>>())).Returns(users.AsAsyncEnumerable());
//         _storageServiceMock.Setup(s => s.GetChatrooms(Any<Func<Chatroom, bool>>(), Any<CancellationToken>(), Any<int>(),
//                                Any<int>(), Any<IQueryOptions<Chatroom>>()))
//                            .Returns(AsyncEnumerable.Empty<Chatroom>());
//
//         var request = new CreateChatroomCommand
//                           {
//                               Usernames = users.Select(u => u.Username).ToList(), Type = ChatType.Private,
//                           };
//
//         var handler = new CreateChatroomHandler(
//             _storageServiceMock.Object,
//             _usersServiceMock.Object
//         );
//
//         var result = await handler.Handle(request, CancellationToken.None);
//
//         Assert.False(result.Created);
//         Assert.Equal(result.ChatId, Guid.Empty);
//         _storageServiceMock.Verify(s => s.AddChatroomAsync(Any<Chatroom>(), Any<CancellationToken>()),
//             Times.Never());
//     }
//
//     [Fact]
//     public async void CreateChatroom_PublicWithNoName_DoesntCreateChatroom()
//     {
//         var users = CreateNUsers(10).ToList();
//         var handler = new CreateChatroomHandler(
//             _storageServiceMock.Object,
//             _usersServiceMock.Object);
//         var request = new CreateChatroomCommand
//                           {
//                               Usernames = users.Select(u => u.Username).ToList(), Type = ChatType.Public
//                           };
//
//         var result = await handler.Handle(request, CancellationToken.None);
//         Assert.False(result.Created);
//         Assert.Equal(result.ChatId, Guid.Empty);
//         _storageServiceMock.Verify(s => s.AddChatroomAsync(Any<Chatroom>(), Any<CancellationToken>()),
//             Times.Never());
//     }
//
//     private static IEnumerable<User> CreateNUsers(int count)
//     {
//         return Enumerable.Range(1, count).Select(n => new User(n.ToString(), n.ToString()));
//     }
//
//     private static T Any<T>()
//     {
//         return It.IsAny<T>();
//     }
// }
//
// class HttpContextAccessorMock : IHttpContextAccessor
// {
//     public HttpContext? HttpContext { get; set; } = new DefaultHttpContext { User = new ClaimsPrincipal() };
// }