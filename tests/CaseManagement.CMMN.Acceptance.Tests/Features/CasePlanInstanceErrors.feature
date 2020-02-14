﻿Feature: CasePlanInstanceErrors
	Check errors returned by /case-plan-instances

Scenario: Create unknown case definition and check error is returned	
	When execute HTTP POST JSON request 'http://localhost/case-plan-instances'
	| Key                | Value |
	| case_definition_id | 1     |
	And extract JSON from body

	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case definition doesn't exist'

Scenario: Launch unknown case instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plan-instances/1/launch'	
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case instance doesn't exist'

Scenario: Get unknown case instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plan-instances/1'
	And extract JSON from body
	
	Then HTTP status code equals to '404'

Scenario: Suspend unknown case instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plan-instances/1/suspend'
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case instance doesn't exist'

Scenario: Suspend unknown case element instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plans/search?case_plan_id=CaseWithOneTask'
	And extract JSON from body
	And extract 'content[0].id' from JSON body into 'defid'
	And execute HTTP POST JSON request 'http://localhost/case-plan-instances'
	| Key          | Value   |
	| case_plan_id | $defid$ |
	And extract JSON from body
	And extract 'id' from JSON body into 'insid'
	And execute HTTP GET request 'http://localhost/case-plan-instances/$insid$/suspend/1'
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case instance element doesn't exist'

Scenario: Reactivate unknown case instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plan-instances/1/reactivate'
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case instance doesn't exist'

Scenario: Reactivate unknown case element instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plans/search?case_plan_id=CaseWithOneTask'
	And extract JSON from body
	And extract 'content[0].id' from JSON body into 'defid'
	And execute HTTP POST JSON request 'http://localhost/case-plan-instances'
	| Key          | Value   |
	| case_plan_id | $defid$ |
	And extract JSON from body
	And extract 'id' from JSON body into 'insid'
	And execute HTTP GET request 'http://localhost/case-plan-instances/$insid$/reactivate/1'
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case instance element doesn't exist'

Scenario: Reactivate not failed case instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plans/search?case_plan_id=CaseWithOneTask'
	And extract JSON from body
	And extract 'content[0].id' from JSON body into 'defid'
	And execute HTTP POST JSON request 'http://localhost/case-plan-instances'
	| Key          | Value   |
	| case_plan_id | $defid$ |
	And extract JSON from body
	And extract 'id' from JSON body into 'insid'
	And execute HTTP GET request 'http://localhost/case-plan-instances/$insid$/reactivate'
	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'errors.transition[0]'='case instance is not completed / terminated / failed / suspended'

Scenario: Activate unknown case instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plan-instances/1/activate/1'
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case activation doesn't exist'

Scenario: Activate unknown case element instance and check error is returned
	When execute HTTP GET request 'http://localhost/case-plans/search?case_plan_id=CaseWithOneManualActivationTask'
	And extract JSON from body
	And extract 'content[0].id' from JSON body into 'defid'
	And execute HTTP POST JSON request 'http://localhost/case-plan-instances'
	| Key          | Value   |
	| case_plan_id | $defid$ |
	And extract JSON from body
	And extract 'id' from JSON body into 'insid'
	And execute HTTP GET request 'http://localhost/case-plan-instances/$insid$/activate/1'
	And extract JSON from body
	
	Then HTTP status code equals to '404'
	Then JSON 'errors.bad_request[0]'='case activation doesn't exist'