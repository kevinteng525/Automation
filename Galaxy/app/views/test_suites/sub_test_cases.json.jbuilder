json.array!(@test_cases) do |test_case|
  json.title test_case.name
  json.lazy false
  json.folder false
  json.tooltip test_case.description
  json.key 'c-' + test_case.id.to_s
  json.url test_case_url(test_case, format: :json)
end
