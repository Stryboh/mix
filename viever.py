import sqlite3


def get_database_tables(file_path: str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
    tables = cursor.fetchall()
    connection.close()
    return tables

def get_table_columns(file_path : str, table_name : str) -> tuple[list, list] | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute(f"SELECT name, type FROM pragma_table_info('{table_name}')").fetchall()
    connection.close()
    columns = []
    types = []
    for column in data:
        columns.append(column[0])
        types.append(column[1])
    return columns, types

def get_table_data(file_path : str, table_name : str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute("SELECT * FROM " + str(table_name)).fetchall()
    connection.close()
    return data

def get_structure(file_path : str) -> str:
    structure = file_path.split('/')[-1] + '\n'
    tables = get_database_tables(file_path)
    for table in tables:
        if str(table)[2: -3] == "sqlite_sequence" or str(table)[2: -3] == "sqlite_master":
            continue
        table = str(table)[2:-3]
        structure += "\n    " + str(table)
        columns, types = get_table_columns(file_path, table)
        for i in range(len(columns)):
            structure += "\n        " + columns[i] + " : " + types[i]
        structure += "\n"
    return structure


print(get_database_tables("your_database.db"))
print()
print()
print()
print(get_table_columns("your_database.db", "your_table"))
print()
print()
print()
print(get_table_data("your_database.db", "your_table"))
print()
print()
print()
print(get_structure("your_database.db"))
