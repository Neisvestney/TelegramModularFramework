---
title: TypeReaders
uid: Guides.TypeReaders
---

# TypeReaders

TypeReaders used to convert string arguments to destination type 

## Available type readers
- @TelegramModularFramework.Services.TypeReaders.StringTypeReader
- @TelegramModularFramework.Services.TypeReaders.IntTypeReader
- @TelegramModularFramework.Services.TypeReaders.BooleanTypeReader
- @TelegramModularFramework.Services.TypeReaders.FloatTypeReader
- @TelegramModularFramework.Services.TypeReaders.DoubleTypeReader

## Create own type reader
- Implement @TelegramModularFramework.Services.TypeReaders.ITypeReader
- Add typereader as service with `services.AddTransient<ITypeReader, TypeReaderClassName>();`