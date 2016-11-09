json.array!(@test_suites_and_cases) do |item|
  json.title item.name
  json.key 's-' + item.id.to_s
  json.folder item.folder
  json.tooltip 'Id=' + item.id.to_s + ' ,' + item.description
  json.lazy item.lazy
end