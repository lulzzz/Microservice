﻿#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown if there is a jwt validation exception
    /// </summary>
    public class WebApiJwtFilterValidationException: TokenValidationException
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="value">The token claim that triggered the exception.</param>
        public WebApiJwtFilterValidationException(string value) : base(value)
        {
        }
    }
}
