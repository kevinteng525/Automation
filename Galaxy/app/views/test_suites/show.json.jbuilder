json.title @test_suite.name
json.key @test_suite.id
json.isFolder true
json.tooltip @test_suite.name
json.isLazy true
json.description @test_suite.description
json.execution_command @test_suite.execution_command
json.type @test_suite.suite_type
json.url test_suite_url(@test_suite, format: :json)