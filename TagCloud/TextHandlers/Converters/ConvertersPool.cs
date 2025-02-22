﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TagCloud.TextHandlers.Converters;

internal class ConvertersPool : IConvertersPool
{
    private readonly List<Func<string, Result<string>>> converters;

    public ConvertersPool(IEnumerable<IConverter> converters)
    {
        this.converters = new List<Func<string, Result<string>>>();
        foreach (var converter in converters)
        {
            this.converters.Add(converter.Convert);
        }
    }

    public Result<IEnumerable<string>> Convert(IEnumerable<string> words)
    {
        return words
            .AsResult()
            .Then(w => w.Select(Convert))
            .Then(w => w.Where(r => r.IsSuccess))
            .Then(results => results.Select(w => w.Value));
    }

    private Result<string> Convert(string word)
    {
        return converters.Aggregate(word.AsResult(), (wordResult, convert) => wordResult.Then(convert));
    }
}