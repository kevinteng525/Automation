json.array!(@test_suites) do |test_suite|
  json.title test_suite.name
  json.key test_suite.id
  json.folder true
  json.tooltip test_suite.name
  json.lazy true
  json.url test_suite_url(test_suite, format: :json)
end
