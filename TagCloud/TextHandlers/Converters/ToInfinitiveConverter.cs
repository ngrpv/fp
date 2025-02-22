﻿using System.Linq;
using NHunspell;

namespace TagCloud.TextHandlers.Converters;

public class ToInfinitiveConverter : IConverter
{
    private readonly Hunspell hunspell;

    public ToInfinitiveConverter()
    {
        hunspell = new Hunspell("ru_ru.aff", "ru_ru.dic");
    }

    public Result<string> Convert(string original)
    {
        return original.AsResult()
            .Then(w => hunspell.Stem(w).FirstOrDefault() ?? w)
            .RefineError("Stem error");
    }
}