import sqlite3
import argparse

def check_login(login :str, password : str) -> bool:
    connection = sqlite3.connect("credentials.db")
    cursor = connection.cursor()
    user = cursor.execute(f"SELECT login FROM Users WHERE login='{login}' AND password='{password}'").fetchall()
    connection.close()
    if user:
        return True
    else:
        return False

def check_access(login: str, database_id: int) -> bool:
    connection = sqlite3.connect("credentials.db")
    cursor = connection.cursor()
    user = cursor.execute(f"SELECT login FROM Users LEFT JOIN Databases on Users.code = Databases.user_id WHERE Databases.id = {database_id}").fetchall()[0][0]
    connection.close()
    if user == login:
        return True
    else:
        return False

def get_avalable_databases(login : str) -> list:
    connection = sqlite3.connect("credentials.db")
    cursor = connection.cursor()
    databases = cursor.execute(f"SELECT * FROM Users LEFT JOIN Databases ON Users.login=Databases.user_id").fetchall()
    connection.close()
    return databases

def get_database_tables(file_path: str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    tables = cursor.execute("SELECT name FROM sqlite_master WHERE type='table'").fetchall()
    connection.close()
    return tables

def get_table_columns(file_path: str, table_name: str) -> tuple[list, list] | None:
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

def get_table_data(file_path: str, table_name: str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute("SELECT * FROM " + str(table_name)).fetchall()
    connection.close()
    return data

def get_structure(file_path: str) -> str:
    structure = file_path.split('/')[-1] + '\n'
    tables = get_database_tables(file_path)
    for table in tables:
        if str(table)[2:-3] == "sqlite_sequence" or str(table)[2:-3] == "sqlite_master":
            continue
        table = str(table)[2:-3]
        structure += "\n    " + str(table)
        columns, types = get_table_columns(file_path, table)
        for i in range(len(columns)):
            structure += "\n        " + columns[i] + " : " + types[i]
        structure += "\n"
    return structure

def main():
    parser = argparse.ArgumentParser(description="Database Viewer")
    parser.add_argument('function', choices=['check_login', 'check_access', 'get_all_databases', 'get_database_tables', 'get_table_columns', 'get_table_data', 'get_structure'], help='Function to execute')
    parser.add_argument('params', nargs='*', help='Parameters for the function')
    args = parser.parse_args()

    if args.function == 'check_login':
        if len(args.params) != 2:
            print("Usage: python viewer.py check_login <login> <password>")
        else:
            login = args.params[0]
            password = args.params[1]
            result = check_login(login, password)
            print(result)
    elif args.function == 'check_access':
        if len(args.params) != 2:
            print("Usage: python viewer.py check_access <login> <database_id>")
        else:
            login = args.params[0]
            database_id = int(args.params[1])
            result = check_access(login, database_id)
            print(result)
    elif args.function == 'get_all_databases':
        if len(args.params) != 1:
            print("Usage: python viewer.py get_all_databases <login>")
        else:
            login = args.params[0]
            database_id = int(args.params[1])
            result = get_avalable_databases(login)
            print(result)
    elif args.function == 'get_database_tables':
        if len(args.params) != 1:
            print("Usage: python viewer.py get_database_tables <file_path>")
        else:
            file_path = args.params[0]
            result = get_database_tables(file_path)
            print(result)
    elif args.function == 'get_table_columns':
        if len(args.params) != 2:
            print("Usage: python viewer.py get_table_columns <file_path> <table_name>")
        else:
            file_path = args.params[0]
            table_name = args.params[1]
            result = get_table_columns(file_path, table_name)
            print(result)
    elif args.function == 'get_table_data':
        if len(args.params) != 2:
            print("Usage: python viewer.py get_table_data <file_path> <table_name>")
        else:
            file_path = args.params[0]
            table_name = args.params[1]
            result = get_table_data(file_path, table_name)
            print(result)
    elif args.function == 'get_structure':
        if len(args.params) != 1:
            print("Usage: python viewer.py get_structure <file_path>")
        else:
            file_path = args.params[0]
            result = get_structure(file_path)
            print(result)

if __name__ == '__main__':
    main()
