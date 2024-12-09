import credentials
import asyncio
import logging
import sys
import sqlite3

from aiogram import Bot, Dispatcher, html
from aiogram.client.default import DefaultBotProperties
from aiogram.enums import ParseMode
from aiogram.filters import CommandStart
from aiogram.types import Message

TOKEN = credentials.token()

dp = Dispatcher()

def get_credentials_by_code(code: str, file_path: str = "credentials.db") -> tuple[str, str] | None:
    connection = sqlite3.connect(file_path)
    cursor = connection.cursor()
    data = cursor.execute("SELECT login, password FROM Users WHERE code = ?", (code,)).fetchone()
    connection.close()
    if data:
        login, password = data
        return login, password
    else:
        return None

@dp.message(CommandStart())
async def command_start_handler(message: Message) -> None:
    await message.answer(f"Привет, {html.bold(message.from_user.full_name)}! Отправь мне кодовое число для доступа.")

@dp.message()
async def check_code_handler(message: Message) -> None:
    code = message.text.strip()
    credentials = get_credentials_by_code(code)
    if credentials:
        login, password = credentials
        await message.answer(f"Логин: {html.bold(login)}\nПароль: {html.bold(password)}")
    else:
        await message.answer("Кодовое число не найдено. Попробуйте снова.")

async def main() -> None:
    bot = Bot(token=TOKEN, default=DefaultBotProperties(parse_mode=ParseMode.HTML))
    await dp.start_polling(bot)


if __name__ == "__main__":
    logging.basicConfig(level=logging.INFO, stream=sys.stdout)
    asyncio.run(main())

