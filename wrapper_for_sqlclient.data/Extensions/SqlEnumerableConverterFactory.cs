using wrapper_for_sqlclient.data.Sql;
using SimpleExtensions;
using System;
using System.Collections.Generic;
using System.Data;

namespace wrapper_for_sqlclient.data.Extensions
{

    public class SqlEnumerableConverterFactory
    {
        public static IEnumerable<SqlEnumerableParameterConverter> Get()
        {
            //We indicate the types in here that are included in the SQL service. 
            //For example in Management Studio: Programmability> Types> User-Defined Table Types. 
            //This is necessary so that when transferring information to the SQL service, you can pass the type as a parameter.
            return new[] {
                new SqlEnumerableParameterConverter {
                    TypeName = "[dbo].[ids]",
                    CType = typeof(IEnumerable<int>),
                    Convert = value => new DataTable()
                            .ColumnAdd<int>("id")
                            .Fill(value as IEnumerable<int>)
                },
                new SqlEnumerableParameterConverter {
                    TypeName="[dbo].[strings]",
                    CType = typeof(IEnumerable<string>),
                    Convert = value => new DataTable()
                        .ColumnAdd<string>("string")
                        .Fill(value as IEnumerable<string>)
                },
                new SqlEnumerableParameterConverter {
                    TypeName = "[dbo].[dates]",
                    CType = typeof(IEnumerable<DateTime>),
                    Convert = value => new DataTable()
                        .ColumnAdd<DateTime>("date")
                        .Fill(value as IEnumerable<DateTime>, date => new object[] {date.Date})
                }
            };
        }
    }
}

