// <copyright file="SpecTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace Stubble.Test.Shared.Spec
{
    public class SpecTest : IXunitSerializable
    {
        public string Name { get; set; }

        public string Desc { get; set; }

        public dynamic Data { get; set; }

        public string Template { get; set; }

        public string Expected { get; set; }

        public bool Skip { get; set; }

        public IDictionary<string, string> Partials { get; set; }

        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public void Deserialize(IXunitSerializationInfo info)
        {
            Name = info.GetValue<string>(nameof(Name));
            Desc = info.GetValue<string>(nameof(Desc));
            Data = JsonConvert.DeserializeObject(info.GetValue<string>(nameof(Data)), serializerSettings);
            Template = info.GetValue<string>(nameof(Template));
            Expected = info.GetValue<string>(nameof(Expected));
            Partials = JsonConvert.DeserializeObject<Dictionary<string, string>>(info.GetValue<string>(nameof(Partials)));
            Skip = info.GetValue<bool>(nameof(Skip));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Desc), Desc);
            info.AddValue(nameof(Data), JsonConvert.SerializeObject(Data, serializerSettings));
            info.AddValue(nameof(Partials), JsonConvert.SerializeObject(Partials, serializerSettings));
            info.AddValue(nameof(Template), Template);
            info.AddValue(nameof(Expected), Expected);
            info.AddValue(nameof(Skip), Skip);
        }
    }
}
