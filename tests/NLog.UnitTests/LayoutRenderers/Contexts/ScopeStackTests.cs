﻿// 
// Copyright (c) 2004-2020 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace NLog.UnitTests.LayoutRenderers
{
    using Xunit;

    public class ScopeStackTests : NLogTestBase
    {
        [Fact]
        public void ScopeStackTest()
        {
            // Arrange
            ScopeContext.Clear();
            var logFactory = new LogFactory();
            logFactory.Setup().LoadConfigurationFromXml(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${scopestack} ${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug' />
                </rules>
            </nlog>");
            var logger = logFactory.GetCurrentClassLogger();
            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");

            // Act
            using (logger.PushScopeState("ala"))
            {
                using (logger.PushScopeState("ma"))
                {
                    logger.Debug("b");
                }
            }

            // Assert
            Assert.Equal("ala ma b", target.LastMessage);
        }

        [Fact]
        public void ScopeStackTopTwoTest()
        {
            // Arrange
            ScopeContext.Clear();
            var logFactory = new LogFactory();
            logFactory.Setup().LoadConfigurationFromXml(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${scopestack:topframes=2} ${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug' />
                </rules>
            </nlog>");
            var logger = logFactory.GetCurrentClassLogger();
            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");

            // Act
            using (logger.PushScopeState("ala"))
            {
                using (logger.PushScopeState("ma"))
                {
                    using (logger.PushScopeState("kota"))
                    {
                        logger.Debug("c");
                    }
                }
            }

            // Assert
            Assert.Equal("ma kota c", target.LastMessage);
        }

        [Fact]
        public void ScopeStackTopOneTest()
        {
            // Arrange
            ScopeContext.Clear();
            var logFactory = new LogFactory();
            logFactory.Setup().LoadConfigurationFromXml(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${scopestack:topframes=1} ${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug' />
                </rules>
            </nlog>");
            var logger = logFactory.GetCurrentClassLogger();
            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");

            // Act
            using (logger.PushScopeState("ala"))
            {
                using (logger.PushScopeState("ma"))
                {
                    using (logger.PushScopeState("kota"))
                    {
                        logger.Debug("c");
                    }
                }
            }

            // Assert
            Assert.Equal("kota c", target.LastMessage);
        }

        [Fact]
        public void ScopeStackBottomTwoTest()
        {
            // Arrange
            ScopeContext.Clear();
            var logFactory = new LogFactory();
            logFactory.Setup().LoadConfigurationFromXml(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${scopestack:bottomframes=2} ${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug' />
                </rules>
            </nlog>");
            var logger = logFactory.GetCurrentClassLogger();
            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");

            // Act
            using (logger.PushScopeState("ala"))
            {
                using (logger.PushScopeState("ma"))
                {
                    using (logger.PushScopeState("kota"))
                    {
                        logger.Debug("c");
                    }
                }
            }

            // Assert
            Assert.Equal("ala ma c", target.LastMessage);
        }

        [Fact]
        public void ScopeStackSeparatorTest()
        {
            // Arrange
            ScopeContext.Clear();
            var logFactory = new LogFactory();
            logFactory.Setup().LoadConfigurationFromXml(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${scopestack:separator=\:} ${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug' />
                </rules>
            </nlog>");
            var logger = logFactory.GetCurrentClassLogger();
            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");

            // Act
            using (logger.PushScopeState("ala"))
            {
                using (logger.PushScopeState("ma"))
                {
                    using (logger.PushScopeState("kota"))
                    {
                        logger.Debug("c");
                    }
                }
            }

            // Assert
            Assert.Equal("ala:ma:kota c", target.LastMessage);
        }

#if NETSTANDARD
        [Fact]
        public void ScopeStackTimingTest()
        {
            // Arrange
            ScopeContext.Clear();
            var logFactory = new LogFactory();
            logFactory.Setup().LoadConfigurationFromXml(@"
            <nlog>
                <targets><target name='debug' type='Debug' layout='${ndlc}|${scopetiming:CurrentScope=false:StartTime=true:Format=yyyy-MM-dd HH\:mm\:ss}|${scopetiming:CurrentScope=false:StartTime=false:Format=fff}|${scopetiming:CurrentScope=true:StartTime=true:Format=HH\:mm\:ss.fff}|${scopetiming:CurrentScope=true:StartTime=false:Format=fffffff}|${message}' /></targets>
                <rules>
                    <logger name='*' minlevel='Debug' writeTo='debug' />
                </rules>
            </nlog>");
            var logger = logFactory.GetCurrentClassLogger();
            var target = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");

            logger.Debug("0");
            string messageNoScope = target.LastMessage;
            string messageFirstScope;
            string messageFirstScopeSleep;
            string messageFirstScopeExit;
            string messageSecondScope;
            string messageSecondScopeSleep;
            Assert.Equal("|||||0", messageNoScope);

            using (logger.PushScopeState("ala"))
            {
                logger.Debug("a");
                messageFirstScope = target.LastMessage;

                System.Threading.Thread.Sleep(10);

                logger.Debug("b");
                messageFirstScopeSleep = target.LastMessage;

                using (logger.PushScopeState("ma"))
                {
                    logger.Debug("a");
                    messageSecondScope = target.LastMessage;

                    System.Threading.Thread.Sleep(10);

                    logger.Debug("b");
                    messageSecondScopeSleep = target.LastMessage;
                }

                logger.Debug("c");
                messageFirstScopeExit = target.LastMessage;
            }

            // Assert
            var measurements = messageFirstScope.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(6, measurements.Length);
            Assert.Equal("ala", measurements[0]);
            Assert.InRange(int.Parse(measurements[2]), 0, 999);
            Assert.InRange(int.Parse(measurements[4]), 0, 9999999);
            Assert.Equal("a", measurements[measurements.Length - 1]);

            measurements = messageFirstScopeSleep.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal("ala", measurements[0]);
            Assert.InRange(int.Parse(measurements[2]), 10, 999);
            Assert.InRange(int.Parse(measurements[4]), 100000, 9999999);
            Assert.Equal("b", measurements[measurements.Length - 1]);

            measurements = messageSecondScope.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(6, measurements.Length);
            Assert.Equal("ala ma", measurements[0]);
            Assert.InRange(int.Parse(measurements[2]), 10, 999);
            Assert.InRange(int.Parse(measurements[4]), 0, 9999999);
            Assert.Equal("a", measurements[measurements.Length - 1]);

            measurements = messageSecondScopeSleep.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(6, measurements.Length);
            Assert.Equal("ala ma", measurements[0]);
            Assert.InRange(int.Parse(measurements[2]), 20, 999);
            Assert.InRange(int.Parse(measurements[4]), 100000, 9999999);
            Assert.Equal("b", measurements[measurements.Length - 1]);

            measurements = messageFirstScopeExit.Split(new[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal("ala", measurements[0]);
            Assert.InRange(int.Parse(measurements[2]), 20, 999);
            Assert.InRange(int.Parse(measurements[4]), 200000, 9999999);
            Assert.Equal("c", measurements[measurements.Length - 1]);
        }
#endif
    }
}