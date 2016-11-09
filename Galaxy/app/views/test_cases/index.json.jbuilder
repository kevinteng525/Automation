json.array!(@test_cases) do |test_case|
  json.title test_case.name
  json.lazy false
  json.folder false
  json.tooltip test_case.name
  json.key test_case.id
  json.url test_case_url(test_case, format: :json)
end
