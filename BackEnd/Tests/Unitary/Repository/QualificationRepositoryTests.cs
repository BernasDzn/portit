
// using Api.Domain.Entities;
// using Api.Domain.ValueObjects;
// using Api.Infrastructure.Persistence;
// using Api.Infrastructure.Persistence.Repositories;
// using Microsoft.EntityFrameworkCore;

// namespace Tests.Unitary.Repositories;

// public class QualificationRepositoryTest
// {
//     private readonly ApiContext _context;
//     private readonly QualificationRepository _repository;

//     public QualificationRepositoryTest()
//     {
 //         var options = new DbContextOptionsBuilder<ApiContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;

//         _context = new ApiContext(options);
//         _repository = new QualificationRepository(_context);
//     }

//     [Fact]
//     public async Task GetQualificationsAsync_ReturnsAllQualifications()
//     {
//         var q1 = new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification 1" });
//         var q2 = new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Qualification 2" });
//         _context.Qualifications.AddRange(q1, q2);
//         await _context.SaveChangesAsync();

//         var result = await _repository.GetQualificationsAsync();
//         Assert.Equal(2, result.Count());
//     }

//     [Fact]
//     public async Task GetQualificationByIdAsync_ReturnsCorrectEntity()
//     {
//         var q = new Qualification(Guid.NewGuid(), new Code { Value = "QX" }, new Designation { Value = "Qualification X" });
//         _context.Qualifications.Add(q);
//         await _context.SaveChangesAsync();

//         var result = await _repository.GetQualificationByIdAsync(q.NameCode.Value);

//         Assert.NotNull(result);
//         Assert.Equal("QX", result!.NameCode.Value);
//     }

//     [Fact]
//     public async Task Add_PersistsQualification()
//     {
//         var q = new Qualification(Guid.NewGuid(), new Code { Value = "QA" }, new Designation { Value = "Qualification A" });

//         var added = await _repository.Add(q);

//         Assert.NotNull(added);
//         Assert.Equal("QA", added.NameCode.Value);
//         Assert.Single(_context.Qualifications);
//     }

//     [Fact]
//     public async Task Update_ChangesEntityData()
//     {
//         var q = new Qualification(Guid.NewGuid(), new Code { Value = "QB" }, new Designation { Value = "Qualification B" });
//         _context.Qualifications.Add(q);
//         await _context.SaveChangesAsync();

//         q.UpdateQualificationName("New Name");

//         var updated = await _repository.Update(q);

//         Assert.Equal("New Name", updated.QualificationName.Value);
//         Assert.Equal("New Name", _context.Qualifications.First().QualificationName.Value);
//     }

//     [Fact]
//     public async Task FilterQualificationsAsync_FiltersByName()
//     {
//         _context.Qualifications.AddRange(
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Alpha" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Beta" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q3" }, new Designation { Value = "Gamma" })
//         );
//         await _context.SaveChangesAsync();

//         var filter = new Api.Application.DataTransfer.Filters.QualificationFilter
//         {
//             QualificationName = "Alpha",
//             PageNumber = 1,
//             PageSize = 10
//         };

//         var result = await _repository.FilterQualificationsAsync(filter);

//         Assert.Single(result.Items);
//         Assert.Equal("Alpha", result.Items.First().QualificationName.Value);
//     }

//     [Fact]
//     public async Task FilterQualificationsAsync_FiltersByCode()
//     {
//         _context.Qualifications.AddRange(
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Alpha" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Beta" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q3" }, new Designation { Value = "Gamma" })
//         );
//         await _context.SaveChangesAsync();

//         var filter = new Api.Application.DataTransfer.Filters.QualificationFilter
//         {
//             Code = "Q2",
//             PageNumber = 1,
//             PageSize = 10
//         };

//         var result = await _repository.FilterQualificationsAsync(filter);

//         Assert.Single(result.Items);
//         Assert.Equal("Q2", result.Items.First().NameCode.Value);
//     }

//     [Fact]
//     public async Task FilterQualificationsAsync_ReturnsAllWhenNoFilter()
//     {
//         _context.Qualifications.AddRange(
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Alpha" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Beta" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q3" }, new Designation { Value = "Gamma" })
//         );
//         await _context.SaveChangesAsync();

//         var filter = new Api.Application.DataTransfer.Filters.QualificationFilter
//         {
//             PageNumber = 1,
//             PageSize = 10
//         };

//         var result = await _repository.FilterQualificationsAsync(filter);

//         Assert.Equal(3, result.Items.Count());
//     }

//     [Fact]
//     public async Task FilterQualificationsAsync_FiltersByNameAndCode()
//     {
//         _context.Qualifications.AddRange(
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q1" }, new Designation { Value = "Alpha" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q2" }, new Designation { Value = "Beta" }),
//             new Qualification(Guid.NewGuid(), new Code { Value = "Q3" }, new Designation { Value = "AlphaBeta" })
//         );
//         await _context.SaveChangesAsync();

//         var filter = new Api.Application.DataTransfer.Filters.QualificationFilter
//         {
//             Code = "Q",
//             QualificationName = "h",
//             PageNumber = 1,
//             PageSize = 10
//         };

//         var result = await _repository.FilterQualificationsAsync(filter);

//         Assert.Equal(2, result.Items.Count());
//         Assert.All(result.Items, q => Assert.Contains("h", q.QualificationName.Value, StringComparison.OrdinalIgnoreCase));
//         Assert.All(result.Items, q => Assert.Contains("Q", q.NameCode.Value, StringComparison.OrdinalIgnoreCase));
//     }

//     [Fact]
//     public async Task FilterQualificationsAsync_PaginatesResults()
//     {
//         for (int i = 1; i <= 25; i++)
//         {
//             _context.Qualifications.Add(new Qualification(Guid.NewGuid(), new Code { Value = $"Q{i}" }, new Designation { Value = $"Qualification {i}" }));
//         }
//         await _context.SaveChangesAsync();

//         var filter = new Api.Application.DataTransfer.Filters.QualificationFilter
//         {
//             PageNumber = 2,
//             PageSize = 10
//         };

//         var result = await _repository.FilterQualificationsAsync(filter);
//         Assert.Equal(10, result.Items.Count());
//         Assert.Equal("Q11", result.Items.First().NameCode.Value);
//     }
// }