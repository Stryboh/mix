# mix

# pip install sqlite3

usage:
  get_database_tables(file_path: str) -> list | None
    input:
      path to database
    output:
      list of all tables in database, None if no tables exist

  get_table_columns(file_path : str, table_name : str) -> tuple[list, list] | None
    input:
      path to database, title of table
    output:
      list of titles of columns in the table, list of data types of columns in the table

  get_table_data(file_path : str, table_name : str) -> list | None
    input:
      path to database, title of table
    output:
      list of lists, storing all the data in the table

  get_structure(file_path : str) -> str
    input:
      path to database
    output:
      string, storing structure of database in format:

        database.db
          table1
            column1
            column2
            column3
          table2
            column1
            column2
            column3
