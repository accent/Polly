﻿using System;
using FluentAssertions;
using Polly.Specs.Helpers;
using Xunit;

namespace Polly.Specs
{
    public class PolicyKeySpecs
    {
        #region Configuration

        [Fact]
        public void Should_be_able_fluently_to_configure_the_policy_key()
        {
            var policy = Policy.Handle<Exception>().Retry().WithPolicyKey(Guid.NewGuid().ToString());

            policy.Should().BeAssignableTo<Policy>();
        }

        [Fact]
        public void PolicyKey_property_should_be_the_fluently_configured_policy_key()
        {
            const string key = "SomePolicyKey";

            var policy = Policy.Handle<Exception>().Retry().WithPolicyKey(key);

            policy.PolicyKey.Should().Be(key);
        }

        [Fact]
        public void Should_not_be_able_to_configure_the_policy_key_explicitly_more_than_once()
        {
            var policy = Policy.Handle<Exception>().Retry();

            Action configure = () => policy.WithPolicyKey(Guid.NewGuid().ToString());

            configure.ShouldNotThrow();

            configure.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("policyKey");
        }

        [Fact]
        public void PolicyKey_property_should_be_non_null_or_empty_if_not_explicitly_configured()
        {
            var policy = Policy.Handle<Exception>().Retry();

            policy.PolicyKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PolicyKey_property_should_start_with_policy_type_if_not_explicitly_configured()
        {
            var policy = Policy.Handle<Exception>().Retry();

            policy.PolicyKey.Should().StartWith("Retry");
        }

        [Fact]
        public void PolicyKey_property_should_be_unique_for_different_instances_if_not_explicitly_configured()
        {
            var policy1 = Policy.Handle<Exception>().Retry();
            var policy2 = Policy.Handle<Exception>().Retry();

            policy1.PolicyKey.Should().NotBe(policy2.PolicyKey);
        }

        [Fact]
        public void PolicyKey_property_should_return_consistent_value_for_same_policy_instance_if_not_explicitly_configured()
        {
            var policy = Policy.Handle<Exception>().Retry();

            var keyRetrievedFirst = policy.PolicyKey;
            var keyRetrievedSecond = policy.PolicyKey;

            keyRetrievedSecond.Should().Be(keyRetrievedFirst);
        }


        [Fact]
        public void Should_not_be_able_to_configure_the_policy_key_explicitly_after_retrieving_default_value()
        {
            var policy = Policy.Handle<Exception>().Retry();

            string retrieveKeyWhenNotExplicitlyConfigured = policy.PolicyKey;

            Action configure = () => policy.WithPolicyKey(Guid.NewGuid().ToString());

            configure.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("p" +
                                                                                 "olicyKey");
        }

        #endregion

        #region PolicyKey and execution Context tests

        [Fact]
        public void Should_pass_PolicyKey_to_execution_context()
        {
            string policyKey = Guid.NewGuid().ToString();

            string policyKeySetOnExecutionContext = null;
            Action<Exception, int, Context> onRetry = (e, i, context) => { policyKeySetOnExecutionContext = context.PolicyKey; };
            var retry = Policy.Handle<Exception>().Retry(1, onRetry).WithPolicyKey(policyKey);

            retry.RaiseException<Exception>(1);

            policyKeySetOnExecutionContext.Should().Be(policyKey);
        }

        [Fact]
        public void Should_pass_ExecutionKey_to_execution_context()
        {
            string executionKey = Guid.NewGuid().ToString();

            string executionKeySetOnContext = null;
            Action<Exception, int, Context> onRetry = (e, i, context) => { executionKeySetOnContext = context.ExecutionKey; };
            var retry = Policy.Handle<Exception>().Retry(1, onRetry);

            bool firstExecution = true;
            retry.Execute(() =>
            {
                if (firstExecution)
                {
                    firstExecution = false;
                    throw new Exception();
                }
            }, new Context(executionKey));

            executionKeySetOnContext.Should().Be(executionKey);
        }

        [Fact]
        public void Should_pass_PolicyKey_to_execution_context_in_generic_execution_on_non_generic_policy()
        {
            string policyKey = Guid.NewGuid().ToString();

            string policyKeySetOnExecutionContext = null;
            Action<Exception, int, Context> onRetry = (e, i, context) => { policyKeySetOnExecutionContext = context.PolicyKey; };
            var retry = Policy.Handle<Exception>().Retry(1, onRetry).WithPolicyKey(policyKey);

            bool firstExecution = true;
            retry.Execute<int>(() =>
            {
                if (firstExecution)
                {
                    firstExecution = false;
                    throw new Exception();
                }
                return 0;
            });

            policyKeySetOnExecutionContext.Should().Be(policyKey);
        }

        [Fact]
        public void Should_pass_ExecutionKey_to_execution_context_in_generic_execution_on_non_generic_policy()
        {
            string executionKey = Guid.NewGuid().ToString();

            string executionKeySetOnContext = null;
            Action<Exception, int, Context> onRetry = (e, i, context) => { executionKeySetOnContext = context.ExecutionKey; };
            var retry = Policy.Handle<Exception>().Retry(1, onRetry);

            bool firstExecution = true;
            retry.Execute<int>(() =>
            {
                if (firstExecution)
                {
                    firstExecution = false;
                    throw new Exception();
                }
                return 0;
            }, new Context(executionKey));

            executionKeySetOnContext.Should().Be(executionKey);
        }
        #endregion
    }


    public class PolicyTResultKeySpecs
    {
        #region Configuration

        [Fact]
        public void Should_be_able_fluently_to_configure_the_policy_key()
        {
            var policy = Policy.HandleResult<int>(0).Retry().WithPolicyKey(Guid.NewGuid().ToString());

            policy.Should().BeAssignableTo<Policy<int>>();
        }

        [Fact]
        public void PolicyKey_property_should_be_the_fluently_configured_policy_key()
        {
            const string key = "SomePolicyKey";

            var policy = Policy.HandleResult(0).Retry().WithPolicyKey(key);

            policy.PolicyKey.Should().Be(key);
        }

        [Fact]
        public void Should_not_be_able_to_configure_the_policy_key_explicitly_more_than_once()
        {
            var policy = Policy.HandleResult(0).Retry();

            Action configure = () => policy.WithPolicyKey(Guid.NewGuid().ToString());

            configure.ShouldNotThrow();

            configure.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("policyKey");
        }

        [Fact]
        public void PolicyKey_property_should_be_non_null_or_empty_if_not_explicitly_configured()
        {
            var policy = Policy.HandleResult(0).Retry();

            policy.PolicyKey.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void PolicyKey_property_should_start_with_policy_type_if_not_explicitly_configured()
        {
            var policy = Policy.HandleResult(0).Retry();

            policy.PolicyKey.Should().StartWith("Retry");
        }

        [Fact]
        public void PolicyKey_property_should_be_unique_for_different_instances_if_not_explicitly_configured()
        {
            var policy1 = Policy.HandleResult(0).Retry();
            var policy2 = Policy.HandleResult(0).Retry();

            policy1.PolicyKey.Should().NotBe(policy2.PolicyKey);
        }

        [Fact]
        public void PolicyKey_property_should_return_consistent_value_for_same_policy_instance_if_not_explicitly_configured()
        {
            var policy = Policy.HandleResult(0).Retry();

            var keyRetrievedFirst = policy.PolicyKey;
            var keyRetrievedSecond = policy.PolicyKey;

            keyRetrievedSecond.Should().Be(keyRetrievedFirst);
        }


        [Fact]
        public void Should_not_be_able_to_configure_the_policy_key_explicitly_after_retrieving_default_value()
        {
            var policy = Policy.HandleResult(0).Retry();

            string retrieveKeyWhenNotExplicitlyConfigured = policy.PolicyKey;

            Action configure = () => policy.WithPolicyKey(Guid.NewGuid().ToString());

            configure.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("policyKey");
        }

        #endregion

        #region PolicyKey and execution Context tests

        [Fact]
        public void Should_pass_PolicyKey_to_execution_context()
        {
            string policyKey = Guid.NewGuid().ToString();

            string policyKeySetOnExecutionContext = null;
            Action<DelegateResult<ResultPrimitive>, int, Context> onRetry = (outcome, i, context) => { policyKeySetOnExecutionContext = context.PolicyKey; };
            var retry = Policy.HandleResult(ResultPrimitive.Fault).Retry(1, onRetry).WithPolicyKey(policyKey);

            retry.RaiseResultSequence(ResultPrimitive.Fault, ResultPrimitive.Good);

            policyKeySetOnExecutionContext.Should().Be(policyKey);
        }

        [Fact]
        public void Should_pass_ExecutionKey_to_execution_context()
        {
            string executionKey = Guid.NewGuid().ToString();

            string executionKeySetOnContext = null;
            Action<DelegateResult<ResultPrimitive>, int, Context> onRetry = (outcome, i, context) => { executionKeySetOnContext = context.ExecutionKey; };
            var retry = Policy.HandleResult(ResultPrimitive.Fault).Retry(1, onRetry);

            bool firstExecution = true;
            retry.Execute(() =>
            {
                if (firstExecution)
                {
                    firstExecution = false;
                    return ResultPrimitive.Fault;
                }
                return ResultPrimitive.Good;
            }, new Context(executionKey));

            executionKeySetOnContext.Should().Be(executionKey);
        }

        #endregion
    }
}
