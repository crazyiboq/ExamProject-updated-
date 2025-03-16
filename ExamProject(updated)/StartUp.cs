using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamProject_updated_
{
    internal class StartUp
    {
        private readonly QuizDbContext _context;
        private static void SeedDatabase(QuizDbContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                );
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "General Knowledge" },
                    new Category { Name = "Science" },
                    new Category { Name = "Math" }
                );
            }

            if (!context.Users.Any(u => u.Role.Name == "Admin"))
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                if (adminRole != null)
                {
                    context.Users.Add(new User
                    {
                        Username = "admin",
                        Password = "admin123",
                        Birthday = new DateTime(2000, 1, 1),
                        RoleId = adminRole.Id
                    });
                }
            }

            context.SaveChanges();
        }

        public StartUp()
        {
            _context = new QuizDbContext();
            _context.Database.EnsureCreated();
            using (var context = new QuizDbContext())
            {
                context.Database.EnsureCreated();
                SeedDatabase(context);
            }

        }




        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Quiz System =====");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Console.WriteLine($"Welcome, {user.Username}!");

                if (user.RoleId == 1)
                {
                    Console.WriteLine("You are logged in as an Admin.");
                    AdminMenu();
                }
                else
                {
                    Console.WriteLine("You are logged in as a User.");
                    UserMenu(user.Id);
                }
            }
            else
            {
                Console.WriteLine("Invalid credentials. Try again.");
            }
            Console.ReadKey();
        }


        private void Register()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            Console.Write("Enter birthday (yyyy-mm-dd): ");
            DateTime birthday = DateTime.Parse(Console.ReadLine());

            Console.Write("psst, wanna be admin-? (yes/no): ");
            string response = Console.ReadLine().Trim().ToLower();

            int roleId = (response == "yes") ? 1 : 2;

            var newUser = new User
            {
                Username = username,
                Password = password,
                Birthday = birthday,
                RoleId = roleId
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            Console.WriteLine(roleId == 1 ? "You are now an admin! 🎉" : "You are registered as a regular user.");
            Console.ReadKey();
        }
        #region Admin
        private void AdminMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Admin Panel =====");
                Console.WriteLine("1. Create Test");
                Console.WriteLine("2. Edit Test");
                Console.WriteLine("3. Delete Test");
                Console.WriteLine("4. View Tests");
                Console.WriteLine("5. Logout");
                Console.WriteLine("6. Create Question With Variants");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateTest();
                        break;
                    case "2":
                        EditTest();
                        break;
                    case "3":
                        DeleteTest(_context);
                        break;
                    case "4":
                        ViewTests(_context);
                        break;
                    case "5":
                        Console.WriteLine("Logging out...");
                        return;
                    case "6":
                        CreateVariantsForQuestion();
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }
        private void CreateTest()
        {
            Console.Write("Enter test title: ");
            string title = Console.ReadLine();
            Console.Write("Enter category ID: ");
            int categoryId = int.Parse(Console.ReadLine());

            var newTest = new Test
            {
                Title = title,
                CategoryId = categoryId,
                CreatedAt = DateTime.Now
            };

            _context.Tests.Add(newTest);
            _context.SaveChanges();

            Console.WriteLine("Test created successfully!");
            Console.ReadKey();
        }

        private void EditTest()
        {
            Console.Clear();
            Console.WriteLine("===== Edit Test =====");

            var tests = _context.Tests.ToList();
            if (tests.Count == 0)
            {
                Console.WriteLine("No tests available.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < tests.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tests[i]}");
            }

            Console.Write("Select a test to edit (by number): ");
            if (!int.TryParse(Console.ReadLine(), out int testIndex) || testIndex < 1 || testIndex > tests.Count)
            {
                Console.WriteLine("Invalid choice!");
                Console.ReadKey();
                return;
            }

            var selectedTest = tests[testIndex - 1];


            Console.Write($"Current Title: {selectedTest.Title}\nEnter new title (or press Enter to keep it the same): ");
            string newTitle = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTitle))
            {
                selectedTest.Title = newTitle;
            }

            Console.Write("How many questions do you want to add to this test? ");
            if (!int.TryParse(Console.ReadLine(), out int questionCount) || questionCount < 1)
            {
                Console.WriteLine("Invalid number of questions.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < questionCount; i++)
            {
                Console.Write("Enter Question ID to add (or type 'new' to create a new one): ");
                string input = Console.ReadLine().Trim();

                if (input.ToLower() == "new")
                {
                    Console.Write("Enter new question: ");
                    string questionText = Console.ReadLine();

                    var newQuestion = new Question { Text = questionText };
                    _context.Questions.Add(newQuestion);
                    _context.SaveChanges();

                    var testQuestion = new TestQuestion { TestId = selectedTest.Id, QuestionId = newQuestion.Id };
                    _context.TestQuestions.Add(testQuestion);
                }
                else if (int.TryParse(input, out int questionId))
                {
                    var existingQuestion = _context.Questions.Find(questionId);
                    if (existingQuestion == null)
                    {
                        Console.WriteLine("Question ID not found.");
                        continue;
                    }

                    var testQuestion = new TestQuestion { TestId = selectedTest.Id, QuestionId = questionId };
                    _context.TestQuestions.Add(testQuestion);
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }
            }

            _context.SaveChanges();
            Console.WriteLine("Test updated successfully!");
            Console.ReadKey();
        }
        public static void DeleteTest(QuizDbContext context)
        {
            Console.WriteLine("Existing Tests:");
            var tests = context.Tests.ToList();

            if (!tests.Any())
            {
                Console.WriteLine("No tests available.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Found {tests.Count} test(s).");


            foreach (var test in tests)
            {
                Console.WriteLine(test);
            }

            Console.Write("Enter the ID of the test to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int testId))
            {
                Console.WriteLine("Invalid input. Please enter a valid test ID.");
                return;
            }

            var testToDelete = context.Tests
                .Include(t => t.TestQuestions)
                .FirstOrDefault(t => t.Id == testId);

            if (testToDelete == null)
            {
                Console.WriteLine("Test not found.");
                Console.ReadKey();
                return;
            }


            context.TestQuestions.RemoveRange(testToDelete.TestQuestions);

            context.Tests.Remove(testToDelete);

            context.SaveChanges();
            Console.WriteLine("Test deleted successfully.");
            Console.ReadKey();
        }
        public void ViewTests(QuizDbContext context)
        {
            Console.Clear();
            Console.WriteLine("===== View Tests =====");

            var tests = context.Tests.ToList();
            if (tests.Count == 0)
            {
                Console.WriteLine("No tests available.");
                Console.ReadKey();
                return;
            }
            foreach (var test in tests) {
                Console.WriteLine(test);
            }
            Console.ReadKey();
        }

        private void CreateVariantsForQuestion()
        {

            var questions = _context.Questions.ToList();
            if (questions.Count == 0)
            {
                Console.WriteLine("No questions found. Please add a question first.");
                return;
            }

            Console.WriteLine("Select a question by ID to add variants:");
            foreach (var q in questions)
            {
                Console.WriteLine($"ID: {q.Id}, Question: {q.Text}");
            }

            Console.Write("Enter Question ID: ");
            if (!int.TryParse(Console.ReadLine(), out int questionId))
            {
                Console.WriteLine("Invalid input. Please enter a valid numeric ID.");
                return;
            }

            var question = _context.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                Console.WriteLine("Question not found.");
                return;
            }


            Console.Write("Enter variant A: ");
            string answerA = Console.ReadLine();
            Console.Write("Enter variant B: ");
            string answerB = Console.ReadLine();
            Console.Write("Enter variant C: ");
            string answerC = Console.ReadLine();
            Console.Write("Enter variant D: ");
            string answerD = Console.ReadLine();

            Console.Write("Which variant is correct? (A/B/C/D): ");
            string correctAnswer = Console.ReadLine().Trim().ToUpper();


            if (correctAnswer != "A" && correctAnswer != "B" && correctAnswer != "C" && correctAnswer != "D")
            {
                Console.WriteLine("Invalid choice for correct answer. Must be A, B, C, or D.");
                return;
            }


            var questionBox = new QuestionBox
            {
                QuestionId = question.Id,
                AnswerA = answerA,
                AnswerB = answerB,
                AnswerC = answerC,
                AnswerD = answerD,
                CorrectAnswer = correctAnswer
            };

            _context.QuestionBoxes.Add(questionBox);
            _context.SaveChanges();

            Console.WriteLine("Variants added successfully to the selected question!");
        }
        #endregion
        #region User
        private void UserMenu(int userId)
        {
            while (true)
            {
                Console.WriteLine("\n===== User Menu =====");
                Console.WriteLine("1. Take a Quiz");
                Console.WriteLine("2. View Leaderboard");
                Console.WriteLine("3. View Exam History");
                Console.WriteLine("4. Update Profile");
                Console.WriteLine("5. Logout");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        TakeQuiz(userId);
                        break;
                    case "2":
                        ViewLeaderboard();
                        break;
                    case "3":
                        ViewExamHistory(userId);
                        break;
                    case "4":
                        ChangePassword(userId);
                        break;
                    case "5":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        private void TakeQuiz(int userId)
        {

            var tests = _context.Tests.Include(t => t.TestQuestions).ThenInclude(q => q.Question).ToList();
            if (tests.Count == 0)
            {
                Console.WriteLine("No tests available.");
                return;
            }

            Console.WriteLine("Available Tests:");
            foreach (var test in tests)
            {
                Console.WriteLine($"ID: {test.Id}, Title: {test.Title}");
            }


            Console.Write("Enter the Test ID to start: ");
            if (!int.TryParse(Console.ReadLine(), out int testId))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            var selectedTest = tests.FirstOrDefault(t => t.Id == testId);
            if (selectedTest == null)
            {
                Console.WriteLine("Test not found.");
                return;
            }


            var testQuestions = selectedTest.TestQuestions.ToList();
            if (testQuestions.Count == 0)
            {
                Console.WriteLine("No questions found for this test.");
                return;
            }

            int score = 0;


            foreach (var testQuestion in testQuestions)
            {
                var questionBox = _context.QuestionBoxes.FirstOrDefault(qb => qb.QuestionId == testQuestion.QuestionId);
                if (questionBox == null) continue;

                Console.WriteLine($"\n{testQuestion.Question.Text}");
                Console.WriteLine($"A) {questionBox.AnswerA}");
                Console.WriteLine($"B) {questionBox.AnswerB}");
                Console.WriteLine($"C) {questionBox.AnswerC}");
                Console.WriteLine($"D) {questionBox.AnswerD}");


                Console.Write("Your answer (A, B, C, D): ");
                string userAnswer = Console.ReadLine().Trim().ToUpper();


                if (userAnswer == questionBox.CorrectAnswer)
                {
                    score++;
                }
            }


            var userTestHistory = new UserTestHistory
            {
                UserId = userId,
                TestId = testId,
                Score = score,
                CompletedAt = DateTime.Now
            };
            _context.UserTestHistories.Add(userTestHistory);
            _context.SaveChanges();


            Console.WriteLine($"\nQuiz Complete! Your Score: {score} / {testQuestions.Count}");
        }

        private void ViewLeaderboard()
        {
            Console.WriteLine("\n===== Leaderboard Menu =====");
            Console.WriteLine("1. View Leaderboard for a Specific Test");
            Console.WriteLine("2. View Global Leaderboard");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewTestLeaderboard();
                    break;
                case "2":
                    ViewGlobalLeaderboard();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Returning to menu.");
                    break;
            }
        }


        private void ViewTestLeaderboard()
        {
            var tests = _context.Tests.ToList();
            if (tests.Count == 0)
            {
                Console.WriteLine("No tests available.");
                return;
            }

            Console.WriteLine("Available Tests:");
            foreach (var test in tests)
            {
                Console.WriteLine($"ID: {test.Id}, Title: {test.Title}");
            }

            Console.Write("Enter the Test ID to view the leaderboard: ");
            if (!int.TryParse(Console.ReadLine(), out int testId))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            var leaderboard = _context.UserTestHistories
                .Where(uth => uth.TestId == testId)
                .OrderByDescending(uth => uth.Score)
                .Take(10)
                .Select(uth => new
                {
                    Username = _context.Users.FirstOrDefault(u => u.Id == uth.UserId).Username,
                    uth.Score,
                    uth.CompletedAt
                })
                .ToList();

            if (leaderboard.Count == 0)
            {
                Console.WriteLine("No results found for this test.");
                return;
            }

            Console.WriteLine("\n===== Test Leaderboard =====");
            foreach (var entry in leaderboard)
            {
                Console.WriteLine($"User: {entry.Username} | Score: {entry.Score} | Completed At: {entry.CompletedAt}");
            }
        }


        private void ViewGlobalLeaderboard()
        {
            var leaderboard = _context.UserTestHistories
                .GroupBy(uth => uth.UserId)
                .Select(group => new
                {
                    Username = _context.Users.FirstOrDefault(u => u.Id == group.Key).Username,
                    TotalScore = group.Sum(uth => uth.Score)
                })
                .OrderByDescending(entry => entry.TotalScore)
                .Take(10)
                .ToList();

            if (leaderboard.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            Console.WriteLine("\n===== Global Leaderboard =====");
            foreach (var entry in leaderboard)
            {
                Console.WriteLine($"User: {entry.Username} | Total Score: {entry.TotalScore}");
            }
        }

        public static void ViewExamHistory(int userId)
        {
            using (var context = new QuizDbContext())
            {
                var history = context.UserTestHistories
             .Include(uth => uth.Test)
             .Where(uth => uth.UserId == userId)
             .OrderByDescending(uth => uth.CompletedAt)
             .ToList();

                if (!history.Any())
                {
                    Console.WriteLine("No exam history found.");
                    return;
                }

                Console.WriteLine("Your Exam History:");
                foreach (var entry in history)
                {
                    int totalQuestions = context.TestQuestions.Count(q => q.TestId == entry.TestId);
                    int maxScore = totalQuestions;
                    Console.WriteLine($"Id - {entry.Test.Id} Test: {entry.Test.Title} | Score: {entry.Score}/{maxScore} | Completed At: {entry.CompletedAt}");
                }


                Console.WriteLine("\nEnter the Test ID to view details (or 0 to go back): ");
                if (int.TryParse(Console.ReadLine(), out int testId) && testId != 0)
                {
                    ViewTestDetails(userId, testId);
                }
            }
        }
        public static void ViewTestDetails(int userId, int testId)
        {
            using (var context = new QuizDbContext())
            {
                var testHistory = context.UserTestHistories
                    .Include(uth => uth.Test)
                    .FirstOrDefault(uth => uth.UserId == userId && uth.TestId == testId);

                if (testHistory == null)
                {
                    Console.WriteLine("Test not found in your history.");
                    return;
                }
                int totalQuestions = context.TestQuestions.Count(q => q.TestId == testId);
                int maxScore = totalQuestions * 10;

                Console.WriteLine($"\nTest: {testHistory.Test.Title}");
                Console.WriteLine($"Score: {testHistory.Score}/{maxScore}");
                Console.WriteLine($"Completed At: {testHistory.CompletedAt}");
            }
        }
        public static void ChangePassword(int userId)
        {
            using (var context = new QuizDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return;
                }

                Console.Write("Enter current password: ");
                string currentPassword = Console.ReadLine();

                if (user.Password != currentPassword)
                {
                    Console.WriteLine("Incorrect password.");
                    return;
                }

                Console.Write("Enter new password: ");
                string newPassword = Console.ReadLine();

                Console.Write("Confirm new password: ");
                string confirmPassword = Console.ReadLine();

                if (newPassword != confirmPassword)
                {
                    Console.WriteLine("Passwords do not match.");
                    return;
                }

                user.Password = newPassword;
                context.SaveChanges();

                Console.WriteLine("Password changed successfully!");
            }
        }


        #endregion

    }
}
