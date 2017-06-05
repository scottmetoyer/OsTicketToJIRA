using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsTicketToJIRA
{
    class Program
    {
        public const string _connectionString = "server=localhost;port=3306;database=osticket;user=root;password=Bumbl3b33";
        public static int _recordsProcessed = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Exporting OSTicket to JSON...");

            _recordsProcessed = 0;
            List<Project> projects = Program.CreateProjects();
            
            var project = projects.Find(x => x.Key == "CYB");
            project.Issues = Program.GetIssues(1, "Cyberinfrastucture");

            project = projects.Find(x => x.Key == "FAC");
            project.Issues = Program.GetIssues(4, "Facilities");

            project = projects.Find(x => x.Key == "CD");
            project.Issues = Program.GetIssues(5, "Catalog Database");

            project = projects.Find(x => x.Key == "COM");
            project.Issues = Program.GetIssues(6, "Communications");

            var export = new Export();
            export.Projects = projects;

            // Export the list to JSON
            Console.Write("Converting to JSON...");
            string json = JsonConvert.SerializeObject(export);
            Console.WriteLine("done.");

            Console.WriteLine("Generating output file.");
            File.WriteAllText("output.json", json);

            Console.WriteLine("Export complete.");
            Console.WriteLine(_recordsProcessed.ToString() + " records processed.");
            Console.ReadKey();
        }

        public static List<Project> CreateProjects()
        {
            Console.Write("Creating top-level JIRA project containers...");
            var projects = new List<Project>();
            projects.Add(new Project
            {
                Key = "CD",
                Name = "Catalog Database"
            });

            projects.Add(new Project
            {
                Key = "COM",
                Name = "Communications"
            });

            projects.Add(new Project
            {
                Key = "CYB",
                Name = "Cyberinfrastructure"
            });

            projects.Add(new Project
            {
                Key = "FAC",
                Name = "Facilities"
            });

            Console.WriteLine("done.");
            return projects;
        }

        public static List<Issue> GetIssues(int departmentId, string department)
        {
            Console.Write("Fetching tickets for " + department + "...");
            var issues = new List<Issue>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // Query the top level tickets in OSTicket
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT * FROM ost_ticket 
                        JOIN ost_user_account ON ost_user_account.user_id = ost_ticket.user_id 
                        JOIN ost_ticket__cdata ON ost_ticket.ticket_id = ost_ticket__cdata.ticket_id
                        WHERE dept_id = " + departmentId.ToString() + " LIMIT 1", conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var i = new Issue()
                        {
                            OsTicketId = reader.GetInt32("ticket_id"),
                            Created = reader.GetDateTime("created"),
                            Reporter = reader.GetString("username"),
                            Assignee = reader.GetString("username"),
                            Resolution = "Resolved",
                            Status = "Resolved",
                            Summary = reader.GetString("subject")
                        };

                        i.CustomFieldValues.Add(new CustomFieldValue { FieldName = "OSTicket Number", Value = reader.GetString("number"), FieldType = "com.atlassian.jira.plugin.system.customfieldtypes:textfield" });
                        issues.Add(i);
                    }
                }
            }

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // Loop to get all the comments
                foreach (var issue in issues)
                {
                    MySqlCommand cmd = new MySqlCommand(
                        @"SELECT * FROM ost_ticket_thread
                            JOIN ost_user_account ON ost_user_account.user_id = ost_ticket_thread.user_id 
                            WHERE ticket_id = " + issue.OsTicketId.ToString() + " ORDER BY created ASC", conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            issue.Comments.Add(new Comment()
                            {
                                Author = reader.GetString("username"),
                                Body = reader.GetString("body"),
                                Created = reader.GetDateTime("created")
                            });
                        }
                    }

                }
            }

            // Set the 

            Console.WriteLine("done. " + issues.Count.ToString() + " records processed.");
            _recordsProcessed += issues.Count;
            return issues;
        }
    }
}