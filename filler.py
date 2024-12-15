import sqlite3
import argparse

def add_data_to_table(file_path: str, table_name: str, columns: list, data: list[list]) -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()

        column_list = "("
        for column in columns:
            column_list += str(column) + ", "
        column_list = column_list[:-2] + ")"

        data_list = ""
        for dat in data:
            data_list += "("
            for i in range(len(dat)):
                data_list += str(dat[i]) + ", "
            data_list = data_list[:-2] + "), "
        data_list = data_list[:-2]
        action = "INSERT INTO " + table_name + " " + column_list + " VALUES " + data_list
        print(action)
        cursor.execute(action)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def erase_data_from_table(file_path: str, table_name: str, condition: str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("DELETE FROM " + table_name + " WHERE " + condition)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def modify_data_in_table(file_path: str, table_name: str, column: str, value: str, condition: str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("UPDATE " + table_name + " SET " + column + "=" + value + " WHERE " + condition)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def set_data_none_in_table(file_path: str, table_name: str, column: str, condition: str = '') -> str | None:
    try:
        connection = sqlite3.connect(file_path)
        cursor = connection.cursor()
        cursor.execute("UPDATE " + table_name + " SET " + column + "=NULL WHERE " + condition)
        connection.commit()
        connection.close()
        return
    except sqlite3.Error as error:
        return str(error)

def main():
    parser = argparse.ArgumentParser(description="Database Filler")
    parser.add_argument('function', choices=['add_data_to_table', 'erase_data_from_table', 'modify_data_in_table', 'set_data_none_in_table'], help='Function to execute')
    parser.add_argument('params', nargs='*', help='Parameters for the function')
    args = parser.parse_args()

    if args.function == 'add_data_to_table':
        if len(args.params) < 4:
            print("Usage: python filler.py add_data_to_table <file_path> <table_name> <columns> <data>")
        else:
            file_path = args.params[0]
            table_name = args.params[1]
            columns = args.params[2].split(',')
            data = [row.split(',') for row in args.params[3:]]
            result = add_data_to_table(file_path, table_name, columns, data)
            if result:
                print(result)

    elif args.function == 'erase_data_from_table':
        if len(args.params) < 2:
            print("Usage: python filler.py erase_data_from_table <file_path> <table_name> [condition]")
        else:
            file_path = args.params[0]
            table_name = args.params[1]
            condition = args.params[2] if len(args.params) > 2 else ''
            result = erase_data_from_table(file_path, table_name, condition)
            if result:
                print(result)

    elif args.function == 'modify_data_in_table':
        if len(args.params) < 4:
            print("Usage: python filler.py modify_data_in_table <file_path> <table_name> <column> <value> [condition]")
        else:
            file_path = args.params[0]
            table_name = args.params[1]
            column = args.params[2]
            value = args.params[3]
            condition = args.params[4] if len(args.params) > 4 else ''
            result = modify_data_in_table(file_path, table_name, column, value, condition)
            if result:
                print(result)

    elif args.function == 'set_data_none_in_table':
        if len(args.params) < 3:
            print("Usage: python filler.py set_data_none_in_table <file_path> <table_name> <column> [condition]")
        else:
            file_path = args.params[0]
            table_name = args.params[1]
            column = args.params[2]
            condition = args.params[3] if len(args.params) > 3 else ''
            result = set_data_none_in_table(file_path, table_name, column, condition)
            if result:
                print(result)

if __name__ == '__main__':
    main()
