import credentials
import asyncio
import logging
import sys

from aiogram import Bot, Dispatcher, html
from aiogram.client.default import DefaultBotProperties
from aiogram.enums import ParseMode
from aiogram.filters import CommandStart
from aiogram.types import Message

TOKEN = credentials.token()

dp = Dispatcher()

def get_credentials_by_code(code: str, file_path: str = "data.txt") -> tuple[str, str] | None:
    with open(file_path, "r", encoding="utf-8") as file:
        lines = file.readlines()
    lines = [line.strip() for line in lines if line.strip()]
    for i in range(len(lines)):
        if lines[i] == code:
            if i + 2 < len(lines):
                login = lines[i + 1]
                password = lines[i + 2]
                return login, password
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

