using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace BigSQLRunner.ConsoleApp
{
    /// <summary>
	/// This indicates that it is start point to run
	/// </summary>
    internal class Program
	{
		/// <summary>
		/// Write error to the log file and console
		/// </summary>
		/// <param name="ex">Exception ex</param>
		/// <param name="logger">TextWriter logger</param>
		/// <param name="enabledLogToFile">bool enabledLogToFile</param>
		private static void WriteExceptionError(Exception ex, TextWriter? logger, bool enabledLogToFile)
		{
			/**********************************************
				If enabling the log, we will write the error
				in Exception to the log file
			**********************************************/
			if (enabledLogToFile && logger != null)
			{
				logger.WriteLine(ex.Message);
			}

			/**********************************************
				Output exception error to console
			**********************************************/
			Console.WriteLine(ex.Message);
		}

		/// <summary>
		/// Print notes to the console before starting to run the sql script
		/// </summary>
		private static void PrintNotes()
		{
			/**********************************************
				Make notes on console screen
			**********************************************/
			Console.WriteLine("NOTES:");
			Console.WriteLine("\t1. To cancel running the sql script. Please press any key to do");
			Console.WriteLine("\t2. To terminate the console. Please press ESC key");
		}

		/// <summary>
		/// Input connection string data
		/// </summary>
		/// <returns>Returns sql connection if okay, otherwise user will continue entering</returns>
		private static SqlConnection InputConnectionStringData()
		{
            /**********************************************
				Connection String Input Data
			**********************************************/
			Console.WriteLine();
			Console.WriteLine("****************** Connection String Input Data ******************");
			do
			{

				/**********************************************
					Read connection string from standard stream
				**********************************************/
				Console.WriteLine("e.g: Server=localhost;Database=DatabaseName;User Id=sa;Password=123;");
				Console.Write("Connection String: ");
				var connectionString = Console.ReadLine();

				try
				{
					/**********************************************
						Open the connection to the database server
					**********************************************/
					var sqlConnection = new SqlConnection(connectionString);
					sqlConnection.Open();

					/**********************************************
						If the connection is in Open status, we
						will break this and go the next.
						Otherwise user will continue entering
					**********************************************/
					if (sqlConnection.State == ConnectionState.Open)
					{
						return sqlConnection;
					}
					else
					{
						Console.WriteLine($"[Error] Your database is in {sqlConnection.State.ToString()} status");
					}
				}
				catch (Exception e)
				{
					/**********************************************
						Output error message to console
					**********************************************/
					Console.WriteLine("[Error] " + e.Message);
				}
			} while (true);
		}

		/// <summary>
		/// Input big sql script file path data
		/// </summary>
		/// <returns>Returns stream reader to file</returns>
		private static StreamReader InputBigSqlScriptFilePathData()
		{
            /**********************************************
				Big Sql Script File Path Input Data
			**********************************************/
			Console.WriteLine();
			Console.WriteLine("****************** Big Sql Script File Path Input Data ******************");
			do
			{

				/**********************************************
					Read the big sql script file path from
					standard stream
				**********************************************/
				Console.WriteLine("e.g: c:\\bigsqlscript.sql");
				Console.Write("Big Sql Script File Path: ");
				var bigSqlScriptFilePath = Console.ReadLine();

                if (string.IsNullOrEmpty(bigSqlScriptFilePath)) continue;
                if (File.Exists(bigSqlScriptFilePath))
                {
                    try
                    {
                        var reader = new StreamReader(bigSqlScriptFilePath);
                        return reader;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[Error] " + e.Message);
                    }
                }
                else
                {
                    Console.WriteLine($"[Error] The big sql script file path '{bigSqlScriptFilePath}' hasn't existed in your hard drive");
                }
            } while (true);
		}

		/// <summary>
		/// Input Enabled log to file data
		/// </summary>
		/// <returns>Returns true if enabled, otherwise false</returns>
		private static bool InputEnabledLogToFileData()
		{
			/**********************************************
				Enabled Log To File Input Data
			**********************************************/
			Console.WriteLine();
			Console.WriteLine("****************** Enabled Log To File Input Data ******************");
			do
			{

				/**********************************************
					Enable Log To File from
					standard stream
				**********************************************/
				Console.WriteLine("e.g: yes or no");
				Console.Write("Enable log to file(yes/no)? ");
				var enabledLogToFileStr = Console.ReadLine();
				enabledLogToFileStr = enabledLogToFileStr != null ? enabledLogToFileStr.Trim().ToLower() : enabledLogToFileStr;

				/**********************************************
					If the user enter "yes" or "no", we will
					enable/disable Log To File, otherwise
					confirm the invalid value to user
				**********************************************/
				if (enabledLogToFileStr == "yes" || enabledLogToFileStr == "no")
				{
					return enabledLogToFileStr == "yes";
				}
                Console.WriteLine("[Error] Please enter either of two values following: yes or no");
            } while (true);
		}

		/// <summary>
		/// Input log file path data
		/// </summary>
		/// <returns>Returns text writer to log file</returns>
		private static TextWriter InputLogFilePathData()
		{
			string? logFilePath;
			TextWriter? logger;

			/**********************************************
				Log File Path Input Data
			**********************************************/
			Console.WriteLine();
			Console.WriteLine("****************** Log File Path Input Data ******************");
			do
			{

				/**********************************************
					Log File Path from
					standard stream
				**********************************************/
				Console.WriteLine("e.g: c:\\log.txt");
				Console.Write("Log File Path: ");
				logFilePath = Console.ReadLine();

				/**********************************************
					If the user enter null or empty or invalid
					extension data, confirm the error message
					to user.
				**********************************************/
				if (string.IsNullOrEmpty(logFilePath) || !logFilePath.EndsWith(".txt"))
				{
					Console.WriteLine("[Error] Please enter valid file path and has extension .txt");
				}
				else
				{
					/**********************************************
						If the log file path existed, break and
						create the stream writer point to that file
					**********************************************/
					if (File.Exists(logFilePath))
					{
						try
						{
							logger = new StreamWriter(logFilePath);
                            break;
                        }
						catch (Exception e)
						{
							Console.WriteLine("[Error] " + e.Message);
						}
					}
					else
					{
						try
						{
							/**********************************************
								If the log file path hasn't existed, create
								new file if okay and create new stream
								writer point to the file stream
							**********************************************/
							var fileStream = File.Create(logFilePath);
                            logger = new StreamWriter(fileStream);
                            break;
                        }
						catch (Exception e)
						{
							/**********************************************
								Inform error message if creating the new
								file unsuccessfully
							**********************************************/
							Console.WriteLine("[Error] " + e.Message);
						}
					}
				}
			} while (true);

			logger.WriteLine($"*************** [{DateTime.Now}]****************");
			logger.WriteLine($"Running {logFilePath}...");
			return logger;
		}

		/// <summary>
		/// Calculate elapsed time in stop watch
		/// </summary>
		/// <param name="stopWatch">stopWatch</param>
		/// <returns>Returns string elapsed time</returns>
		private static string CalElapsedTime(Stopwatch? stopWatch)
		{
            if (stopWatch == null) return string.Empty;
            var timeSpan = stopWatch.Elapsed;
            return $"Time elapsed: {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
        }

        /// <summary>
        /// This is a starting point to run program in command line
        /// </summary>
        private static void Main()
		{
			/**********************************************
				Initialize these necessary input parameters
			**********************************************/
            TextWriter? logger = null;

            PrintNotes();
			var sqlConnection = InputConnectionStringData();
			var reader = InputBigSqlScriptFilePathData();
			var enabledLogToFile = InputEnabledLogToFileData();
			if (enabledLogToFile)
			{
				logger = InputLogFilePathData();
			}

			/**********************************************
				Measure time of running sql script
			**********************************************/
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			try
			{
				/**********************************************
					Contain line code sql script and next line
					one
				**********************************************/
                var scriptDataLine = string.Empty;

                /**********************************************
					Count the number of records added to db
				**********************************************/
                var counter = 0;

				/**********************************************
					Count the number of affected when running
					command sql
				**********************************************/

                /**********************************************
					Initialize the Sql Command to run the sql
					script
				**********************************************/

                /**********************************************
					Initialize the message to show to the user
				**********************************************/

                /**********************************************
					Specify the first times to run the script
				**********************************************/
				var isFirst = true;

				/**********************************************
					Indicate the running status
				**********************************************/
				Console.WriteLine("Running...");

				/**********************************************
					Indicate running the script until EOF
				**********************************************/
				while (reader.ReadLine() is { } nextScriptDataLine && Console.KeyAvailable == false)
				{
					try
					{

						/**********************************************
							Should combine the current line and the new
							line which was read from file to run
						**********************************************/
						scriptDataLine = $"{scriptDataLine} {nextScriptDataLine}";

						/**********************************************
							Should trim the line data to avoid extra
							whitespaces between begin and end string
						**********************************************/
						scriptDataLine = scriptDataLine.Trim();

						/**********************************************
							Run the script right away when seeing GO
							batch. This batch is always sensitive case
						**********************************************/
						if (scriptDataLine.EndsWith("GO"))
						{
							/**********************************************
								Cut off the GO string at the end of the line
								data
							**********************************************/
							scriptDataLine = scriptDataLine.Substring(0, scriptDataLine.Length - 2);

							/**********************************************
								Send the script to the database server to
								run
							**********************************************/
							var sqlCommand = new SqlCommand(scriptDataLine, sqlConnection);
							var numberOfAffectedRows = sqlCommand.ExecuteNonQuery();

							/**********************************************
								Reset the line data to blank value
							**********************************************/
							scriptDataLine = string.Empty;

							/**********************************************
								If having the number of affected rows is
								greater than zero, we will add it into
								counter
							**********************************************/
							if (numberOfAffectedRows > 0)
							{
								counter += numberOfAffectedRows;
							}

							/**********************************************
								Only accept when the counter is greater
								than zero
							**********************************************/
							if (counter > 0)
							{
								/**********************************************
									If this is the first time, we will write
									message to console to update status,
									otherwise, we will re-update static in
									console
								**********************************************/
								if (isFirst)
								{
									var message = $"Added {counter} row(s)";
									Console.Write(message);
									isFirst = false;
								}
								else
								{
									/**********************************************
										Point to position on console screen which
										has the sentence format "Added {0} row(s)"
									**********************************************/
									Console.SetCursorPosition(0, Console.CursorTop);
									Console.Write($"Added {counter} row(s)");
								}
							}
						}
					}
					catch (SqlException ex)
					{
						WriteExceptionError(ex, logger, enabledLogToFile);
						/**********************************************
							Should reset the line data to blank value
							in case previous command SQL sentence causes
							error
						**********************************************/
						scriptDataLine = string.Empty;
					}
					catch (Exception ex)
					{
						WriteExceptionError(ex, logger, enabledLogToFile);
						break;
					}
				}

				/**********************************************
					Confirm that the sql script file were
					finished running. If enabling the log, we
					will write the current status to the log
					file
				**********************************************/
				if (enabledLogToFile && logger != null)
				{
					logger.WriteLine("Completed");
					logger.WriteLine($"Total {counter} rows added to database");
				}
				Console.WriteLine("\nCompleted");
				Console.WriteLine($"Total {counter} rows added to database");

				/**********************************************
					Release unused resources
				**********************************************/
				reader.Close();
				sqlConnection.Close();
			}
			catch (Exception e)
			{
				WriteExceptionError(e, logger, enabledLogToFile);
			}
			finally
			{
				/**********************************************
					Release unused logger
				**********************************************/
				if (enabledLogToFile && logger != null)
				{
					logger.Close();
				}

				/**********************************************
					Stop watch and write the elapsed time to
					Console
				**********************************************/
				stopWatch.Stop();
				Console.WriteLine(CalElapsedTime(stopWatch));

				/**********************************************
					Wait until user enter ENTER key to exit
				**********************************************/
				while (true)
				{
					var consoleKey = Console.ReadKey().Key;
					if (consoleKey == ConsoleKey.Escape)
					{
						break;
					}
				}
			}
		}
	}
}
