json.array!(@subscribed_projects) do |project|
  json.extract! project, :id, :name, :description
  json.url project_url(project, format: :json)
end
