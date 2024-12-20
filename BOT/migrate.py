import sqlite3
import argparse

def intersection(list1 : list, list2 : list) -> list:
    result = []
    for item in list1:
        try:
            list2.remove(item)
            result.append(item)
        except:
            pass
    return result

def get_database_tables(file_path: str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    cursor.execute("SELECT name FROM sqlite_master WHERE type='table'")
    tables = cursor.fetchall()
    connection.close()
    return tables

def get_table_columns(file_path : str, table_title : str) -> list | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute(f"SELECT name, type FROM pragma_table_info('{table_title}')").fetchall()
    connection.close()
    return data

def get_data_type(file_path : str, table_name : str, column_title) -> str | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    type = cursor.execute(f"SELECT type FROM pragma_table_info('{table_name}') WHERE name='{column_title}'").fetchall()[0][0]
    connection.close()
    return type

def check_connectivity(file_1_path: str, table_1_name: str, column_1_name: str, file_2_path: str, table_2_name: str, column_2_name: str) -> bool:
    try:
        connection1 = sqlite3.connect(file_1_path)
        cursor1 = connection1.cursor()
        type1 = cursor1.execute(f"SELECT type FROM pragma_table_info('{table_1_name}') WHERE name='{column_1_name}'").fetchall()
        connection1.close()

        connection2 = sqlite3.connect(file_2_path)
        cursor2 = connection2.cursor()
        type2 = cursor2.execute(f"SELECT type FROM pragma_table_info('{table_2_name}') WHERE name='{column_2_name}'").fetchall()
        connection2.close()

        return type1 == type2
    except None:
        return False

def transfer_data(old_file_path: str, old_table_name: str, old_column_name: str, new_file_path: str, new_table_name: str, new_column_name: str) -> str | None:
    if check_connectivity(old_file_path, old_table_name, old_column_name, new_file_path, new_table_name, new_column_name):
        connection1 = sqlite3.connect(old_file_path)
        connection2 = sqlite3.connect(new_file_path)
        cursor1 = connection1.cursor()
        cursor2 = connection2.cursor()
        try:
            new_column_data = cursor1.execute(f"SELECT {old_column_name} FROM {old_table_name}").fetchall()
            if get_data_type(old_file_path, old_table_name, old_column_name) == 'TEXT':
                for i in range(len(new_column_data)):
                    new_column_data[i] = f"\'{str(new_column_data[i][0])}\'"
            else:
                for i in range(len(new_column_data)):
                    new_column_data[i] = new_column_data[i][0]
            rows = len(cursor2.execute(f"SELECT {new_column_name} FROM {new_table_name}").fetchall())
            for i in range(len(new_column_data) - rows):
                cursor2.execute(f"INSERT INTO {new_table_name} ({new_column_name}) VALUES (null)")
            action = f"UPDATE {new_table_name} SET {new_column_name}=CASE "
            for i in range(len(new_column_data)):
                action += f"WHEN rowid={i+1} THEN {new_column_data[i]} "
            action += f"END"
            cursor2.execute(action)

            connection2.commit()
            connection1.close()
            connection2.close()
        except sqlite3.Error as error:
            connection1.close()
            connection2.close()
            return str(error)
    else:
        return "Columns are not compatible"

def auto_transfer_data(old_file_path : str, new_file_path : str) -> list:
    old_tables = get_database_tables(old_file_path)
    new_tables = get_database_tables(new_file_path)

    old_columns = []
    new_columns = []

    for table in old_tables:
        columns = get_table_columns(old_file_path, table[0])
        for column in columns:
            old_columns.append((table[0], (column[0], column[1])))

    for table in new_tables:
        columns = get_table_columns(new_file_path, table[0])
        for column in columns:
            new_columns.append((table[0], (column[0], column[1])))

    columns = intersection(old_columns, new_columns)
    columns_copied = []
    for column in columns:
        table_name, column_data = column
        column_name, column_type = column_data
        columns_copied.append(transfer_data(old_file_path, table_name, column_name, new_file_path, table_name, column_name))
    return columns_copied

def main():
    parser = argparse.ArgumentParser(description="Database Migration")
    parser.add_argument('function', choices=['check_connectivity', 'transfer_data', 'auto_transfer_data'], help='Function to execute')
    parser.add_argument('params', nargs='*', help='Parameters for the function')
    args = parser.parse_args()

    if args.function == 'check_connectivity':
        if len(args.params) != 6:
            print("Usage: python migrate.py check_connectivity <file_1_path> <table_1_name> <column_1_name> <file_2_path> <table_2_name> <column_2_name>")
        else:
            file_1_path = args.params[0]
            table_1_name = args.params[1]
            column_1_name = args.params[2]
            file_2_path = args.params[3]
            table_2_name = args.params[4]
            column_2_name = args.params[5]
            result = check_connectivity(file_1_path, table_1_name, column_1_name, file_2_path, table_2_name, column_2_name)
            print(result)

    elif args.function == 'transfer_data':
        if len(args.params) != 6:
            print("Usage: python migrate.py transfer_data <old_file_path> <old_table_name> <old_column_name> <new_file_path> <new_table_name> <new_column_name>")
        else:
            old_file_path = args.params[0]
            old_table_name = args.params[1]
            old_column_name = args.params[2]
            new_file_path = args.params[3]
            new_table_name = args.params[4]
            new_column_name = args.params[5]
            result = transfer_data(old_file_path, old_table_name, old_column_name, new_file_path, new_table_name, new_column_name)
            if result:
                print(result)

    elif args.function == 'auto_transfer_data':
        if len(args.params) != 2:
            print("Usage: python migrate.py auto_transfer_data <old_file_path> <new_file_path>")
        else:
            old_file_path = args.params[0]
            new_file_path = args.params[1]
            result = auto_transfer_data(old_file_path, new_file_path)
            print(result)

if __name__ == '__main__':
    main()
