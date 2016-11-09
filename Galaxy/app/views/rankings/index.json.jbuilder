json.array!(@rankings) do |ranking|
  json.extract! ranking, :id, :name, :description
  json.url ranking_url(ranking, format: :json)
end
