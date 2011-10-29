using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;

namespace DBTests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The time in seconds to wait for commands to execute.
        /// </summary>
        protected abstract int CommandTimeout { get; }

        protected abstract string ConnectionString { get; }

        protected abstract string TestSchema { get; }

        [TestCaseSource("Procs")]
        public void Test_procs_should_return_empty(string proc)
        {
            Assert.That(proc, Is.Not.Null);

            var records = ReadFromDatabase(CommandType.StoredProcedure, proc, rec => rec.GetValue(0));

            var result = records.Take(10).ToArray();

            Assert.That(result, Is.Empty);
        }

        public IEnumerable<string> Procs
        {
            get
            {
                return ReadFromDatabase(CommandType.Text,
                    string.Format("SELECT '['+SPECIFIC_SCHEMA+'].['+SPECIFIC_NAME+']' FROM INFORMATION_SCHEMA.ROUTINES WHERE SPECIFIC_SCHEMA = '{0}'", TestSchema),
                    record => record.GetString(0));
            }
        }

        private IEnumerable<TResult> ReadFromDatabase<TResult>(CommandType cmdType, string cmdText, Func<IDataRecord, TResult> resultSelector)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(cmdText, conn) { CommandType = cmdType, CommandTimeout = CommandTimeout })
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                    yield return resultSelector(reader);
            }
        }
    }
}
