# mix

# pip install sqlite3


get_database_tables(file_path: str) -> list | None

input:
path to database

output:
list of all tables in database, None if no tables exist

# 

get_table_columns(file_path : str, table_name : str) -> tuple[list, list] | None

input:
path to database, title of table

output:
list of titles of columns in the table, list of data types of columns in the table

# 

get_table_data(file_path : str, table_name : str) -> list | None

input:
path to database, title of table

output:
list of lists, storing all the data in the table

# 

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
# 

add_data_to_table(file_path : str, table_name : str, columns : list, data : tuple[list, ], condition : str = '') -> str | None

adds several rows of data into specified rows in the table

input:
path to database, title of table, list titles of columns to place data, data to insert, specification of rows

output:
error string or None

# 

erase_data_from_table(file_path : str, table_name : str, condition : str = '') -> str | None

removes specified rows in the table

input:
path to database, title of table, specification of rows

output:
error string or None

# 

modify_data_in_table(file_path : str, table_name : str, column : str, value : str, condition : str = '') -> str | None

replaces data in the table

input:
path to database, title of table, title of column, new value, specification of rows

output:
error string or None

# 

set_data_none_in_table(file_path : str, table_name : str, column : str, condition : str = '') -> str | None

sets data to None value in the table

input:
path to database, title of table, title of column, specification of rows

output:
error string or None
